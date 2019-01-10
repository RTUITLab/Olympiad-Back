using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Executor.Logging;
using Shared.Models;
using Models.Solutions;
using Models.Exercises;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text.RegularExpressions;

namespace Executor.Executers.Run
{
    abstract class ProgramRunner
    {
        private readonly BlockingCollection<(Guid solutionId, ExerciseData[] testData)> solutionQueue
            = new BlockingCollection<(Guid solutionId, ExerciseData[] testData)>();

        private readonly Func<Guid, SolutionStatus, Task> processSolution;
        private readonly IDockerClient dockerClient;
        private Task runningTask;
        private readonly Logger<ProgramRunner> logger;
        private string lang;
        private string Language => lang ??
                                   (lang = GetType().GetCustomAttribute<LanguageAttribute>().Lang);


        public ProgramRunner(Func<Guid, SolutionStatus, Task> processSolution, IDockerClient dockerClient)
        {
            logger = Logger<ProgramRunner>.CreateLogger(Language);
            runningTask = Task.Run(RunLoop);
            this.processSolution = processSolution;
            this.dockerClient = dockerClient;
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
                result = await Run(imageName, data);
                logger.LogTrace($"check solution {solutionId} in {data.InData} out {data.OutData} result: {result}");
                if (result != SolutionStatus.Sucessful)
                {
                    break;
                }
            }
            await processSolution(solutionId, result);
            await dockerClient.Images.DeleteImageAsync(imageName, new ImageDeleteParameters { Force = true, PruneChildren = true });
        }

        protected async Task<SolutionStatus> Run(string imageName, ExerciseData testData)
        {
            var container = await dockerClient.Containers.CreateContainerAsync(new Docker.DotNet.Models.CreateContainerParameters
            {
                Image = imageName,
                OpenStdin = true
            });
            SolutionStatus status = SolutionStatus.InProcessing;
            try
            {
                status = await RunAndCheck(container.ID, testData);
            }
            catch (Exception ex)
            {
                logger.LogWarning("error while checking solution", ex);
            }
            await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters { Force = true });
            return status;
        }

        private async Task<SolutionStatus> RunAndCheck(string containerId, ExerciseData testData)
        {
            var started = await dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters { });
            if (!started)
                throw new Exception($"Cant start container {containerId}");
            var stream = await dockerClient.Containers.AttachContainerAsync(containerId, false, new ContainerAttachParameters { Stream = true, Stdin = true, Stderr = true, Stdout = true });

            var inStream = new MemoryStream(Encoding.UTF8.GetBytes(testData.InData + '\n'));
            await stream.CopyFromAsync(inStream, CancellationToken.None);
            var readTask = stream.ReadOutputToEndAsync(CancellationToken.None);

            if (await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(5)), readTask) == readTask)
            {
                var (stdout, stderr) = readTask.Result;
                if (!string.IsNullOrEmpty(stderr))
                    return SolutionStatus.RunTimeError;
                stdout = Regex.Replace(stdout, @"[\u0000-\u001F]+", string.Empty);
                if (string.Equals(stdout, testData.OutData.Trim()))
                    return SolutionStatus.Sucessful;
                return SolutionStatus.WrongAnswer;
            }
            return SolutionStatus.TooLongWork;

        }
    }
}