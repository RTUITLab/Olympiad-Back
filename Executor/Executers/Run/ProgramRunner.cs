using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Olympiad.Shared.Models;
using Models.Exercises;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PublicAPI.Requests;
using System.Collections.Generic;
using Executor.Models.Settings;
using System.Security.Cryptography.X509Certificates;

namespace Executor.Executers.Run
{
    class ProgramRunner
    {
        private readonly ISolutionsBase solutionsBase;
        private readonly IDockerClient dockerClient;
        private readonly RunningSettings runningSettings;
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
            ILogger<ProgramRunner> logger)
        {
            this.solutionsBase = solutionsBase;
            this.dockerClient = dockerClient;
            this.runningSettings = runningSettings;
            this.logger = logger;
        }
        public async Task RunAndCheckSolution(Guid solutionId, ExerciseData[] testData)
        {
            if (blackList.Contains(solutionId))
            {
                // TODO what?
                logger.LogError($"solution from black list {solutionId}");
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


        private async Task HandleSolutionWorker(Guid solutionId, string imageName, ExerciseData[] testData, Action<SolutionStatus> storeSolutionStatus, CancellationToken token)
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
                    else
                        localStatus = SolutionStatus.WrongAnswer;
                    if (duration > TimeSpan.FromSeconds(10))
                        localStatus = SolutionStatus.TooLongWork;

                    await solutionsBase.SaveLog(solutionId, new SolutionCheckRequest
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
            var container = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = imageName,
                OpenStdin = true,
                NetworkDisabled = true,
                StdinOnce = true
            });
            var result = ("", "", TimeSpan.Zero);
            Exception exception = null;
            try
            {
                var (stdout, stderr, duration) = await RunContainer(container.ID, input);
                stdout = Clean(stdout);
                stderr = Clean(stderr);
                result = (stdout, stderr, duration);
            }
            catch (Exception ex)
            {
                logger.LogWarning("error while checking solution", ex);
                exception = ex;
            }
            await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters { Force = true });
            if (exception != null)
                throw exception;
            return result;
        }

        private async Task<(string stdout, string stderr, TimeSpan duration)> RunContainer(string containerId, string input)
        {
            var started = await dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
            if (!started)
                throw new Exception($"Cant start container {containerId}");

            var readStream = await dockerClient.Containers.AttachContainerAsync(containerId, false, new ContainerAttachParameters { Stream = true, Stdin = false, Stderr = true, Stdout = true });
            var writeStream = await dockerClient.Containers.AttachContainerAsync(containerId, false, new ContainerAttachParameters { Stream = true, Stdin = true, Stderr = false, Stdout = false });

            var inStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            var startTime = DateTime.UtcNow;
            await writeStream.CopyFromAsync(inStream, CancellationToken.None);
            writeStream.CloseWrite();
            var readTask = readStream.ReadOutputToEndAsync(CancellationToken.None);

            if (await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(5)), readTask) == readTask)
            {
                var (stdout, stderr) = readTask.Result;
                return (stdout, stderr, DateTime.UtcNow - startTime);
            }
            return ("", "", TimeSpan.MaxValue); // TODO read all output streams
        }

        private static string Clean(string input)
        {
            input = Regex.Replace(input, @"[\u0000-\u0009]+", string.Empty);
            input = Regex.Replace(input, @"[\u000B-\u001F]+", string.Empty).Trim();
            return input;
        }
    }
}