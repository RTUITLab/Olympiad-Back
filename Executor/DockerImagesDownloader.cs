using Docker.DotNet;
using Docker.DotNet.Models;
using Executor.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Executor
{
    public class DockerImagesDownloader
    {
        private readonly IDockerClient dockerClient;
        private readonly IOptions<StartSettings> options;
        private readonly ILogger<DockerImagesDownloader> logger;

        public DockerImagesDownloader(
            IDockerClient dockerClient,
            IOptions<StartSettings> options,
            ILogger<DockerImagesDownloader> logger)
        {
            this.dockerClient = dockerClient;
            this.options = options;
            this.logger = logger;
        }
        public async Task DownloadBaseImages()
        {
            var dockerFileInfos = Directory
                .GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Executers", "Build", "DockerFiles"))
                .Select(f => (imageName: File.ReadAllLines(f)[0]["FROM ".Length..], lang: Path.GetFileName(f)["Dockerfile-".Length..]))
                .ToList();
            var imageRegex = new Regex("([^:]+):([^:]+)");
            foreach (var (imageName, lang) in dockerFileInfos)
            {
                var parsed = imageRegex.Match(imageName);
                var name = parsed.Groups[1].Value;
                var tag = parsed.Groups[2].Value;

                logger.LogInformation($"Creating image for {lang}");
                if (options.Value.PrivateDockerRegistry != null)
                {
                    var privateName = options.Value.PrivateDockerRegistry.Address + "/" + name;
                    var authConfig = new AuthConfig
                    {
                        Username = options.Value.PrivateDockerRegistry.Login,
                        Password = options.Value.PrivateDockerRegistry.Password,
                        ServerAddress = options.Value.PrivateDockerRegistry.Address
                    };
                    try
                    {
                        await PullImage(privateName, tag, authConfig);
                    }
                    catch (DockerApiException dex) when (dex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        logger.LogInformation($"Image {privateName}:{tag} is not present, pushing");
                        await PullImage(name, tag);
                        await dockerClient.Images.TagImageAsync($"{name}:{tag}", new ImageTagParameters
                        {
                            RepositoryName = $"{privateName}",
                            Tag = tag
                        });
                        await PushImage(privateName, tag, authConfig);
                    }
                }
                else
                {
                    await PullImage(name, tag);
                }
            }
        }

        private async Task PullImage(string name, string tag, AuthConfig authConfig = null)
        {
            var progress = new Progress<JSONMessage>();
            progress.ProgressChanged += (s, m) =>
            {
                logger.LogInformation($"pull {name}:{tag} {m.ID} {m.Status} {m.ProgressMessage}");
            };
            await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = name,
                Tag = tag
            }, authConfig, progress);
        }

        private async Task PushImage(string name, string tag, AuthConfig authConfig = null)
        {
            var progress = new Progress<JSONMessage>();
            progress.ProgressChanged += (s, m) =>
            {
                logger.LogInformation($"push {name}:{tag} {m.ID} {m.Status} {m.ProgressMessage}");
            };
            await dockerClient.Images.PushImageAsync(name, new ImagePushParameters
            {
                Tag = tag
            }, authConfig, progress);
        }
    }
}
