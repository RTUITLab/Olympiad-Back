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
using System.Text.RegularExpressions;

namespace Executor.Executers.Build
{
    class ProgramBuilder
    {
        private readonly Func<Guid, SolutionStatus, Task> processSolution;
        private readonly Func<Guid, string, Task> saveBuildLogs;
        private readonly BuildProperty buildProperty;
        private readonly IDockerClient dockerClient;
        private readonly ILogger<ProgramBuilder> logger;



        public int BuildQueueLength => 0;


        private DateTime currentStart;
        public Solution Current { get; private set; }
        public TimeSpan CurrentBuildTime => DateTime.Now - currentStart;

        public ProgramBuilder(
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Guid, string, Task> saveBuildLogs,
            BuildProperty buildProperty,
            IDockerClient dockerClient,
            ILogger<ProgramBuilder> logger)
        {
            this.processSolution = processSolution;
            this.saveBuildLogs = saveBuildLogs;
            this.buildProperty = buildProperty;
            this.dockerClient = dockerClient;
            this.logger = logger;
        }

        public async Task<bool> BuildSolution(Solution solution)
        {
            Current = solution;
            currentStart = DateTime.Now;
            solution.Status = SolutionStatus.InProcessing;
            await processSolution(solution.Id, SolutionStatus.InProcessing);
            logger.LogInformation($"build solution {solution.Id}");
            try
            {
                var (buildSuccess, buildLogs) = await BuidSource(solution.Id, solution.Language, solution.Raw);
                await saveBuildLogs(solution.Id, buildLogs);

                if (!buildSuccess)
                {
                    await processSolution(solution.Id, SolutionStatus.CompileError);
                    logger.LogInformation($"{solution.Id} {SolutionStatus.CompileError}");
                    return false;
                }
                logger.LogInformation($"{solution.Id} BUILDED");
                return true;
            }
            catch (Exception ex)
            {
                await processSolution(solution.Id, SolutionStatus.ErrorWhileCompile);
                logger.LogWarning($"Build solution {solution.Id} with error", ex);
                return false;
            }
            finally
            {
                Current = null;
            }
        }

        protected async Task<(bool, string)> BuidSource(Guid solutionId, string lang, string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return (false, "EMPTY SOLUTION");
            }
            var sourceDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            logger.LogDebug($"new dir is {sourceDir.FullName}");

            File.WriteAllText(Path.Combine(sourceDir.FullName, buildProperty.ProgramFileName), raw, new UTF8Encoding(false));
            var dockerFile = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Executers", "Build", "DockerFiles", $"DockerFile-{lang}"));
            File.WriteAllText(Path.Combine(sourceDir.FullName, "DockerFile"), dockerFile, new UTF8Encoding(false));

            var buildLogs = await BuildImageAsync($"solution:{solutionId}", sourceDir.FullName);
            sourceDir.Delete(true);
            logger.LogDebug(buildLogs);
            return (!buildProperty.IsCompilationFailed(buildLogs), buildLogs);
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
