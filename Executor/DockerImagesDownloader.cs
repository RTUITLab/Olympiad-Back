using Docker.DotNet;
using Docker.DotNet.Models;
using Executor.Models.Settings;
using Executor.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Executor;

public partial class DockerImagesDownloader(
    IDockerClient dockerClient,
    IOptions<StartSettings> options,
    ILogger<DockerImagesDownloader> logger)
{
    public async Task DownloadBaseImages()
    {
        var dockerFileInfos = Directory
            .GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Executers", "Build", "DockerFiles"))
            .Select(f => (imageName: File.ReadAllLines(f)[0]["FROM ".Length..], lang: Path.GetFileName(f)["Dockerfile-".Length..]))
            .ToList();
        foreach (var (imageName, lang) in dockerFileInfos)
        {
            var (name , tag) = ImageNameParser.Parse(imageName);

            logger.LogInformation("Creating image for {Language}", lang);
            if (options.Value.PrivateDockerRegistry != null)
            {
                var privateName = options.Value.PrivateDockerRegistry.Address + "/" + name.Replace(".", "-").Replace("/", "-"); // replace for work with deep images
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
                    logger.LogInformation("Image {PrivateImageName}:{PrivateImageTag} is not present, pushing", privateName, tag);
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
            logger.LogInformation("Pull {ImageName}:{ImageTag} {MessageId} {MessageStatus} {MessageProgressMessage}", name, tag, m.ID, m.Status, m.ProgressMessage);
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
            logger.LogInformation("Push {ImageName}:{ImageTag} {MessageId} {MessageStatus} {MessageProgressMessage}", name, tag, m.ID, m.Status, m.ProgressMessage);
        };
        await dockerClient.Images.PushImageAsync(name, new ImagePushParameters
        {
            Tag = tag
        }, authConfig, progress);
    }
}
