﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Models;
using Models.Exercises;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PublicAPI.Requests;

namespace Executor.Executers.Run
{
    class ProgramRunner
    {
        private readonly BlockingCollection<(Guid solutionId, ExerciseData[] testData)> solutionQueue
            = new BlockingCollection<(Guid solutionId, ExerciseData[] testData)>();

        private readonly ISolutionsBase solutionsBase;
        private readonly IDockerClient dockerClient;
        private readonly ILogger<ProgramRunner> logger;
        private Task runningTask;



        public ProgramRunner(
            ISolutionsBase solutionsBase,
            IDockerClient dockerClient,
            ILogger<ProgramRunner> logger)
        {
            runningTask = Task.Run(RunLoop);
            this.solutionsBase = solutionsBase;
            this.dockerClient = dockerClient;
            this.logger = logger;
        }

        public void Add(Guid solutionId, ExerciseData[] data)
        {
            if (solutionQueue.Any(s => s.solutionId == solutionId)) return;
            logger.LogInformation($"add solution {solutionId}, data count: {data.Length}");
            solutionQueue.Add((solutionId, data));
        }
        private async Task RunLoop()
        {
            await Task.Yield();
            foreach (var (solutionId, testData) in solutionQueue.GetConsumingEnumerable())
            {
                await HandleSolution(solutionId, testData);
            }
        }

        private async Task HandleSolution(Guid solutionId, ExerciseData[] testData)
        {
            var result = SolutionStatus.Sucessful;
            var imageName = $"solution:{solutionId}";

            foreach (var data in testData)
            {
                var exampleIn = data.InData + '\n';
                var exampleOut = data.OutData.Trim();
                var (stdout, stderr, duration) = await Run(imageName, exampleIn);
                await solutionsBase.SaveLog(solutionId, new SolutionCheckRequest
                {
                    ExampleIn = exampleIn,
                    ExampleOut = exampleOut,
                    ProgramOut = stdout,
                    ProgramErr = stderr,
                    Duration = duration
                });
                SolutionStatus localStatus;
                if (!string.IsNullOrEmpty(stderr))
                    localStatus = SolutionStatus.RunTimeError;
                if (string.Equals(stdout, exampleOut))
                    localStatus = SolutionStatus.Sucessful;
                else
                    localStatus = SolutionStatus.WrongAnswer;

                logger.LogTrace($"check solution {solutionId} in {data.InData} out {data.OutData} result: {localStatus}");
                if (localStatus < result)
                    result = localStatus;
            }
            await solutionsBase.SaveChanges(solutionId, result);
            await dockerClient.Images.DeleteImageAsync(imageName, new ImageDeleteParameters { Force = true, PruneChildren = true });
        }

        private async Task<(string stdout, string stderr, TimeSpan duration)> Run(string imageName, string input)
        {
            var container = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = imageName,
                OpenStdin = true,
                NetworkDisabled = true
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
            var startTime = DateTime.UtcNow;
            var started = await dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
            if (!started)
                throw new Exception($"Cant start container {containerId}");
            var stream = await dockerClient.Containers.AttachContainerAsync(containerId, false, new ContainerAttachParameters { Stream = true, Stdin = true, Stderr = true, Stdout = true });

            var inStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            await stream.CopyFromAsync(inStream, CancellationToken.None);
            var readTask = stream.ReadOutputToEndAsync(CancellationToken.None);


            if (await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(5)), readTask) == readTask)
            {
                var (stdout, stderr) = readTask.Result;
                return (stdout, stderr, DateTime.UtcNow - startTime);
            }
            return ("", "", TimeSpan.MaxValue); // read all output streams
        }

        private static string Clean(string input)
        {
            input = Regex.Replace(input, @"[\u0000-\u0009]+", string.Empty);
            input = Regex.Replace(input, @"[\u000B-\u001F]+", string.Empty).Trim();
            return input;
        }
    }
}