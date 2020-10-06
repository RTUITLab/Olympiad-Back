using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Executor.Models.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Olympiad.Shared.Models.Settings;

namespace Executor
{
    class Program
    {
        private const string SettingsFileName = "appsettings.Secret.json";
        private static IConfiguration configuration;

        static async Task Main(string[] args)
        {
            Console.WriteLine($"Started {DateTime.Now}");
            try
            {

                configuration = SetupConfigs(args);

                var servicesProvider = BuildServices();

                if (!await IsDockerAvailable(servicesProvider.GetRequiredService<IDockerClient>()))
                {
                    throw new Exception("host must see docker!");
                }

                var executor = servicesProvider.GetRequiredService<Executor>();
                var statusReporter = servicesProvider.GetRequiredService<ConsoleStatusReporter>();
                var statusReporterTask = configuration.GetConsoleMode() == ConsoleMode.StatusReporting ?
                    statusReporter.Start(executor, CancellationToken.None)
                    :
                    Task.CompletedTask;

                await DownloadBaseImages(
                    servicesProvider.GetRequiredService<IDockerClient>(),
                    servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DockerImagesDownloader"));


                await Task.WhenAll(
                    executor.Start(CancellationToken.None),
                    statusReporterTask);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                Console.Error.WriteLine($"Exited {DateTime.Now}");
                Environment.Exit(1);
            }
        }


        private static async Task DownloadBaseImages(
            IDockerClient dockerClient,
            ILogger logger)
        {
            var dockerFileInfos = Directory
                .GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Executers", "Build", "DockerFiles"))
                .Select(f => (imageName: File.ReadAllLines(f)[0].Substring("FROM ".Length), lang: Path.GetFileName(f).Substring("Dockerfile-".Length)))
                .ToList();
            var imageRegex = new Regex("([^:]+):([^:]+)");
            foreach (var (imageName, lang) in dockerFileInfos)
            {
                var parsed = imageRegex.Match(imageName);
                var name = parsed.Groups[1].Value;
                var tag = parsed.Groups[2].Value;

                var progress = new Progress<JSONMessage>();
                logger.LogInformation($"Creating image for {lang}");
                progress.ProgressChanged += (s, m) =>
                {
                    logger.LogInformation($"{name}:{tag} {m.ID} {m.Status} {m.ProgressMessage}");
                };
                await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
                {
                    FromImage = name,
                    Tag = tag
                }, null, progress);

            }

        }

        private static async Task<bool> IsDockerAvailable(IDockerClient dockerClient)
        {
            try
            {
                await dockerClient.System.PingAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static IServiceProvider BuildServices()
            => new ServiceCollection()
                .AddLogging(configure =>
                {
                    if (configuration.GetConsoleMode() == ConsoleMode.Logs)
                    {
                        configure.AddConsole();
                    }
                    configure.AddProvider(new ConsoleStatusReporterLoggerProvider());
                    configure.AddConfiguration(configuration.GetSection("Logging"));
                })
                .Configure<StartSettings>(configuration.GetSection(nameof(StartSettings)))
                .Configure<UserInfo>(configuration.GetSection(nameof(UserInfo)))
                .ConfigureAndValidate<RabbitMqQueueSettings>(configuration.GetSection(nameof(RabbitMqQueueSettings)))
                .ConfigureAndValidate<RunningSettings>(configuration.GetSection(nameof(RunningSettings)))
                .AddTransient<ISolutionsBase, DbManager>()
                .AddSingleton<Executor>()
                .AddSingleton<ConsoleStatusReporter>()
                .AddHttpClient(DbManager.DbManagerHttpClientName, (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<StartSettings>>();
                    client.BaseAddress = new Uri(options.Value.Address);
                })
                .Services
                .AddSingleton<IDockerClient>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<StartSettings>>();
                    return new DockerClientConfiguration(new Uri(options.Value.DockerEndPoint)).CreateClient();
                })
                .BuildServiceProvider();

        private static IConfiguration SetupConfigs(string[] args)
            => new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile(SettingsFileName, optional: true)
                .AddEnvironmentVariables()
                .Build();
    }
}
