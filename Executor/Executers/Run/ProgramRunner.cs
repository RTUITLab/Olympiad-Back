using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Olympiad.Shared.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PublicAPI.Requests;
using System.Collections.Generic;
using Executor.Models.Settings;
using PublicAPI.Responses.ExercisesTestData;
using System.Buffers;
using Executor.Utils;

namespace Executor.Executers.Run
{
    partial class ProgramRunner
    {
        private readonly ISolutionsBase solutionsBase;
        private readonly IDockerClient dockerClient;
        private readonly RunningSettings runningSettings;
        private readonly AutoDeleteTempFileProvider autoDeleteTempFileProvider;
        private readonly ILogger<ProgramRunner> logger;
        private readonly List<Guid> blackList = new List<Guid>();


        public Guid? Current { get; private set; }
        private int currentTestDataIndex = -1;
        private int currentTestDataChecked;
        public int CurrentTestDataCheckedCount => currentTestDataChecked;
        public int CurrentTestDataCount { get; private set; }
        private DateTime currentStart;

        public TimeSpan CurrentBuildTime => DateTime.Now - currentStart;


        public ProgramRunner(
            ISolutionsBase solutionsBase,
            IDockerClient dockerClient,
            RunningSettings runningSettings,
            AutoDeleteTempFileProvider autoDeleteTempFileProvider,
            ILogger<ProgramRunner> logger)
        {
            this.solutionsBase = solutionsBase;
            this.dockerClient = dockerClient;
            this.runningSettings = runningSettings;
            this.autoDeleteTempFileProvider = autoDeleteTempFileProvider;
            this.logger = logger;
        }
        public async Task RunAndCheckSolution(Guid solutionId, ExerciseDataResponse[] testData)
        {
            if (blackList.Contains(solutionId))
            {
                // TODO what?
                logger.LogError("solution from black list {SolutionId}", solutionId);
                return;
            }

            CancellationTokenSource source = new CancellationTokenSource();
            Task[] workTasks = null;
            try
            {
                currentStart = DateTime.Now;
                CurrentTestDataCount = testData.Length;
                Current = solutionId;
                var imageName = $"solution:{solutionId}";
                ConcurrentQueue<SolutionStatus> statuses = new ConcurrentQueue<SolutionStatus>();
                workTasks = Enumerable
                    .Range(1, runningSettings.WorkersPerCheckCount)
                    .Select(_ =>
                        HandleSolutionWorker(
                            solutionId,
                            imageName,
                            testData,
                            (s) => statuses.Enqueue(s),
                            source.Token))
                    .ToArray();
                await Task.WhenAll(workTasks);
                var result = statuses.DefaultIfEmpty(SolutionStatus.Successful).Min();
                logger.LogInformation($"{solutionId} TOTAL STATUS {result}");
                await solutionsBase.SaveChanges(solutionId, result);
                await dockerClient.Images.DeleteImageAsync(imageName, new ImageDeleteParameters { Force = true, NoPrune = false });
            }
            catch (Exception ex)
            {
                blackList.Add(solutionId);
                logger.LogError(ex, "Critical error");
                source.Cancel();
                if (workTasks != null)
                {
                    foreach (var task in workTasks.Where(t => t != null))
                    {
                        task.ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                logger.LogError(t.Exception, "Critical error on worker");
                            }
                        }).Wait();
                    }
                }
            }
            finally
            {
                Current = null;
                currentTestDataIndex = -1;
                currentTestDataChecked = 0;
                CurrentTestDataCount = 0;
            }
        }


        private async Task HandleSolutionWorker(Guid solutionId, string imageName, ExerciseDataResponse[] testData, Action<SolutionStatus> storeSolutionStatus, CancellationToken token)
        {
            while (CurrentTestDataCheckedCount < CurrentTestDataCount && !token.IsCancellationRequested)
            {
                var index = Interlocked.Increment(ref currentTestDataIndex);
                if (index >= CurrentTestDataCount)
                {
                    return;
                }
                var data = testData[index];

                var exampleIn = data.InData + '\n';
                var exampleOut = data.OutData?.Trim() ?? "";
                try
                {
                    var (stdout, stderr, duration) = await Run(imageName, exampleIn);
                    SolutionStatus localStatus;

                    if (!string.IsNullOrEmpty(stderr))
                        localStatus = SolutionStatus.RunTimeError;
                    else if (string.Equals(stdout, exampleOut))
                        localStatus = SolutionStatus.Successful;
                    else if (duration > TimeSpan.FromSeconds(5))
                        localStatus = SolutionStatus.TooLongWork;
                    else
                        localStatus = SolutionStatus.WrongAnswer;

                    await solutionsBase.SaveLog(solutionId, data.Id, new SolutionCheckRequest
                    {
                        ExampleIn = exampleIn,
                        ExampleOut = exampleOut,
                        ProgramOut = stdout,
                        ProgramErr = stderr,
                        Duration = duration,
                        Status = localStatus
                    });

                    logger.LogTrace($"check solution {solutionId} in {data.InData} out {data.OutData} result: {localStatus}");
                    logger.LogInformation($"{solutionId} checked on {data.Id} {localStatus}");
                    storeSolutionStatus(localStatus);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "error while running");
                    storeSolutionStatus(SolutionStatus.RunTimeError);
                }
                finally
                {
                    Interlocked.Increment(ref currentTestDataChecked);
                }
            }
        }

        private async Task<(string stdout, string stderr, TimeSpan duration)> Run(string imageName, string input)
        {
            using var tempFile = autoDeleteTempFileProvider.GetTempFile();
            await File.WriteAllTextAsync(tempFile.LocalFilePath, input);
            var container = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = imageName,
                OpenStdin = true,
                NetworkDisabled = true,
                StdinOnce = true,
                HostConfig = new HostConfig
                {
                    AutoRemove = true,
                    Binds = [$"{tempFile.HostFilePath}:/var/input_data"]
                }
            });
            try
            {
                var (stdout, stderr, duration) = await RunContainer(container.ID);
                stdout = Clean(stdout);
                stderr = Clean(stderr);
                return (stdout, stderr, duration);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "error while checking solution");
                throw;
            }
        }

        private async Task<(string stdout, string stderr, TimeSpan duration)> RunContainer(string containerId)
        {
            var started = await dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
            if (!started)
                throw new Exception($"Can't start container {containerId}");

            var logs = await dockerClient.Containers.GetContainerLogsAsync(containerId, false, new ContainerLogsParameters { ShowStderr = true, ShowStdout = true, Follow = true });
            var startTime = DateTime.UtcNow;

            var containerWaitTask = dockerClient.Containers.WaitContainerAsync(containerId);
            await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(5)), containerWaitTask);

            var duration = DateTime.UtcNow - startTime;
            try
            {
                var inspect = await dockerClient.Containers.InspectContainerAsync(containerId);
                if (inspect.State.Running)
                {
                    await dockerClient.Containers.KillContainerAsync(containerId, new ContainerKillParameters());
                }
            }
            catch (DockerContainerNotFoundException) { } // контейнер уже умер, это хорошо
            catch (DockerApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                logger.LogDebug(ex, "container already killed");
            }
            var (stdout, stderr) = await logs.ReadOutputToEndAsync(CancellationToken.None);
            return (stdout, stderr, duration);
        }

        private static string Clean(string input)
        {
            input = CleanRegexRange1().Replace(input, string.Empty);
            input = CleanRegexRange2().Replace(input, string.Empty).Trim();
            return input;
        }

        [GeneratedRegex(@"[\u0000-\u0009]+")]
        private static partial Regex CleanRegexRange1();
        [GeneratedRegex(@"[\u000B-\u001F]+")]
        private static partial Regex CleanRegexRange2();
    }
}