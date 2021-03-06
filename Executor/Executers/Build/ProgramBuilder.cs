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
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Executor.Models.Settings;
using PublicAPI.Requests;
using System.Text.Json;

namespace Executor.Executers.Build
{
    class ProgramBuilder
    {
        private readonly Dictionary<string, BuildProperty> buildProperties = new Dictionary<string, BuildProperty>
        {
            { "c", new ContainsInLogsProperty { ProgramFileName = "Program.c", BuildFailedCondition = "error" } },
            { "cpp", new ContainsInLogsProperty { ProgramFileName = "Program.cpp", BuildFailedCondition = "error" } },
            { "csharp", new ContainsInLogsProperty { ProgramFileName = "Program.cs", BuildFailedCondition = "Build FAILED" } },
            { "java", new ContainsInLogsProperty { ProgramFileName = "Main.java", BuildFailedCondition = "error" } },
            { "pasabc", new ContainsInLogsProperty { ProgramFileName = "Program.pas", BuildFailedCondition = "Compile errors:" } },
            { "python", new ContainsInLogsProperty { ProgramFileName = "Program.py", BuildFailedCondition = "error" } },
            { "js", new ContainsInLogsProperty { ProgramFileName = "Program.js", BuildFailedCondition = "error" } },
            { "fpas", new ContainsInLogsProperty { ProgramFileName = "Program.pas", BuildFailedCondition = "error" } },
        };

        private readonly Func<Guid, SolutionStatus, Task> processSolution;
        private readonly Func<Guid, BuildLogRequest, Task> saveBuildLogs;
        private readonly IDockerClient dockerClient;
        private readonly StartSettings startOptions;
        private readonly ILogger<ProgramBuilder> logger;



        private DateTime currentStart;
        public Solution Current { get; private set; }
        public TimeSpan CurrentBuildTime => DateTime.Now - currentStart;

        public ProgramBuilder(
            Func<Guid, SolutionStatus, Task> processSolution,
            Func<Guid, BuildLogRequest, Task> saveBuildLogs,
            IDockerClient dockerClient,
            StartSettings startOptions,
            ILogger<ProgramBuilder> logger)
        {
            this.processSolution = processSolution;
            this.saveBuildLogs = saveBuildLogs;
            this.dockerClient = dockerClient;
            this.startOptions = startOptions;
            this.logger = logger;
        }
        /// <summary>
        /// Clean docker build logs
        /// </summary>
        /// <param name="rawLogs">raw logs from docker build</param>
        /// <returns>Clean logs</returns>
        private static string PrettyLogs(string rawLogs)
        {
            return string.Join("\n", rawLogs
                .Split("\n")
                .Select(row => { try { return JsonDocument.Parse(row).RootElement; } catch { return default; } })
                .Where(e => e.ValueKind == JsonValueKind.Object)
                .Where(d => d.TryGetProperty("stream", out _))
                .Select(d => d.GetProperty("stream").GetString().Trim())
                .Where(d => !d.StartsWith("--->"))
                .Where(d => !d.Contains("Successfully built "))
                .Where(d => !d.Contains("Successfully tagged "))
                .Where(d => !d.Contains("Removing intermediate container"))
                .Select(d => Regex.Replace(d, @"^Step \d+/\d+ : ", ""))
                .Where(d => !d.StartsWith("FROM"))
                );
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
                await saveBuildLogs(solution.Id, new BuildLogRequest
                {
                    RawBuildLog = buildLogs,
                    PrettyBuildLog = PrettyLogs(buildLogs)
                });

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

            var buildProperty = buildProperties[lang];

            var sourceDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            logger.LogDebug($"new dir is {sourceDir.FullName}");

            await PrepareBuildFiles(lang, raw, buildProperty, sourceDir);

            var buildLogs = await BuildImageAsync($"solution:{solutionId}", sourceDir.FullName);
            sourceDir.Delete(true);
            logger.LogDebug(buildLogs);
            return (!buildProperty.IsCompilationFailed(buildLogs), buildLogs);
        }

        private async Task PrepareBuildFiles(string lang, string raw, BuildProperty buildProperty, DirectoryInfo sourceDir)
        {
            await File.WriteAllTextAsync(Path.Combine(sourceDir.FullName, buildProperty.ProgramFileName), raw, new UTF8Encoding(false));
            var dockerFile = await File.ReadAllLinesAsync(Path.Combine(Directory.GetCurrentDirectory(), "Executers", "Build", "DockerFiles", $"DockerFile-{lang}"));
            if (startOptions.PrivateDockerRegistry != null)
            {
                var image = dockerFile[0].Split(' ')[1];
                dockerFile[0] = $"FROM {startOptions.PrivateDockerRegistry.Address}/{image}";
            }
            await File.WriteAllLinesAsync(Path.Combine(sourceDir.FullName, "DockerFile"), dockerFile, new UTF8Encoding(false));
        }

        private async Task<string> BuildImageAsync(string imageName, string buildContext)
        {
            var archivePath = Path.Combine(buildContext, "context.tar.gz");
            await CreateTarGz(archivePath, buildContext);
            using var archStream = File.OpenRead(archivePath);
            var buildParameters = new ImageBuildParameters
            {
                Dockerfile = "DockerFile",
                Tags = new[] { imageName }
            };
            if (startOptions.PrivateDockerRegistry != null) {
                buildParameters.AuthConfigs = new Dictionary<string, AuthConfig>
                {
                    { startOptions.PrivateDockerRegistry.Address,
                        new AuthConfig
                        {
                            Username = startOptions.PrivateDockerRegistry.Login,
                            Password = startOptions.PrivateDockerRegistry.Password,
                        }
                    }
                };
            }

            var outStream = await dockerClient.Images.BuildImageFromDockerfileAsync(archStream, buildParameters);
            using var streamReader = new StreamReader(outStream);
            return await streamReader.ReadToEndAsync();
        }
        private static async Task CreateTarGz(
            string tgzFilename, 
            string sourceDirectory)
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
