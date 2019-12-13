using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Models.Solutions;
using Olympiad.Shared.Models;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Logging;

namespace Executor.Executers.Build
{
    class ProgramBuilder
    {
        private readonly BlockingCollection<Solution> solutionQueue = new BlockingCollection<Solution>();
        private readonly Func<Guid, SolutionStatus, Task> processSolution;
        private readonly Func<Solution, Task> finishBuildSolution;
        private readonly BuildProperty buildProperty;
        private readonly IDockerClient dockerClient;
        private readonly ILogger<ProgramBuilder> logger;

        private Task buildingTask;
        public ProgramBuilder(
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Solution, Task> finishBuildSolution,
            BuildProperty buildProperty,
            IDockerClient dockerClient,
            ILogger<ProgramBuilder> logger)
        {
            buildingTask = Task.Run(BuildLoop);
            this.processSolution = processSolution;
            this.finishBuildSolution = finishBuildSolution;
            this.buildProperty = buildProperty;
            this.dockerClient = dockerClient;
            this.logger = logger;
        }
        public void Add(Solution solution)
        {
            logger.LogDebug($"Add solution {solution.Id}");
            if (solutionQueue.Any(s => s.Id == solution.Id)) return;
            solutionQueue.Add(solution);
        }
        private async Task BuildLoop()
        {
            await Task.Yield();
            foreach (var solution in solutionQueue.GetConsumingEnumerable())
            {
                solution.Status = SolutionStatus.InProcessing;
                await processSolution(solution.Id, SolutionStatus.InProcessing);
                logger.LogInformation($"build solution {solution.Id}");
                try
                {
                    if (!await Build(solution.Id, solution.Language, solution.Raw))
                    {
                        await processSolution(solution.Id, SolutionStatus.CompileError);
                        continue;
                    }
                    await finishBuildSolution(solution);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Build solution {solution.Id} with error", ex);
                }
            }
        }

        protected virtual async Task<bool> Build(Guid solutionId, string lang, string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }
            if (lang == "java")
            {
                //TODO rename Java class to main
            }
            var sourceDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            logger.LogDebug($"new dir is {sourceDir.FullName}");
            File.WriteAllText(Path.Combine(sourceDir.FullName, buildProperty.ProgramFileName), raw, new UTF8Encoding(false));
            var dockerFile = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Executers", "Build", "DockerFiles", $"DockerFile-{lang}"));
            File.WriteAllText(Path.Combine(sourceDir.FullName, "DockerFile"), dockerFile, new UTF8Encoding(false));

            var buildLogs = await BuildImageAsync($"solution:{solutionId}", sourceDir.FullName);

            sourceDir.Delete(true);
            Console.WriteLine(buildLogs);
            return !buildProperty.IsCompilationFailed(buildLogs);
        }

        private async Task<string> BuildImageAsync(string imageName, string buildContext)
        {
            var archivePath = Path.Combine(buildContext, "context.tar.gz");
            await CreateTarGz(archivePath, buildContext);
            using (var archStream = File.OpenRead(archivePath))
            {
                var outStream = await dockerClient.Images.BuildImageFromDockerfileAsync(archStream, new ImageBuildParameters
                {
                    Dockerfile = "DockerFile",
                    Tags = new[] { imageName }
                });
                using (var streamReader = new StreamReader(outStream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }
        private static async Task CreateTarGz(string tgzFilename, string sourceDirectory)
        {
            var outStream = File.Create(tgzFilename);
            var tarOutputStream = new TarOutputStream(outStream);

            var fileNames = Directory.GetFiles(sourceDirectory).Where(f => !f.EndsWith(".tar.gz"));
            foreach (var filename in fileNames)
            {
                using (var fileStream = File.OpenRead(filename))
                {
                    var entry = TarEntry.CreateTarEntry(Path.GetFileName(filename));
                    entry.Size = fileStream.Length;
                    tarOutputStream.PutNextEntry(entry);
                    await fileStream.CopyToAsync(tarOutputStream);
                }
                tarOutputStream.CloseEntry();
            }
            tarOutputStream.Close();
        }
    }
}
