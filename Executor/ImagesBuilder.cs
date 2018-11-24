using Executor.Executers;
using Executor.Executers.Build;
using Executor.Executers.Run;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Executor
{
    class ImagesBuilder
    {
        private readonly IDockerClient dockerClient;

        public ImagesBuilder(IDockerClient dockerClient)
        {
            this.dockerClient = dockerClient;
        }
        public async Task<bool> CheckAndBuildImages()
        {
            if (!await CheckDockerConnection()) return false;

            var needImages = NeedImages().ToArray();
            Console.WriteLine($"ALL NEEDED IMAGES ({needImages.Length})");
            foreach (var needImage in needImages)
            {
                Console.WriteLine(needImage);
            }

            var currentImages = (await CurrentImages()).ToArray();
            Console.WriteLine($"CURRENT IMAGES ({currentImages.Length})");
            foreach (var currentImage in currentImages)
            {
                Console.WriteLine(currentImage);
            }
            var needToBuild = needImages.Except(currentImages).ToList();
            needToBuild.ForEach(Console.WriteLine);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Executers");
            var folderPairs = needToBuild
                .Select(n => (n, folder: n.StartsWith("runner") ? "Run" : "Build"))
                .Select(p => (p.n, folder: Path.Combine(path, p.folder, p.n.Split(":")[1], "DockerFile")))
                .ToList();
            foreach (var (n, folder) in folderPairs)
            {
                Console.WriteLine(n);
                Console.WriteLine(folder);
                await BuildImage(n, folder);
                Console.WriteLine(new string('-', 10));
            }
            return true;
        }

        private async Task BuildImage(string imageName, string dockerFilePath)
        {
            if (!File.Exists(dockerFilePath))
            {
                Console.WriteLine($"file {dockerFilePath} not exists");
                return;
            }

            var archivePath = Path.ChangeExtension(dockerFilePath, ".tar.gz");
            await CreateTarGZ(archivePath, Path.GetDirectoryName(dockerFilePath));

            var outStream = await dockerClient.Images.BuildImageFromDockerfileAsync(File.OpenRead(archivePath), new ImageBuildParameters
            {
                Dockerfile = "DockerFile",
                Tags = new[] { imageName }
            });
            using (var streamReader = new StreamReader(outStream))
            {
                while (!streamReader.EndOfStream)
                {
                    Console.WriteLine(await streamReader.ReadLineAsync());
                }
            }
        }

        private static IEnumerable<string> NeedImages()
        {
            var builders = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(T =>
                    T.BaseType == typeof(ProgramBuilder))
                .Select(T => T.GetCustomAttribute<LanguageAttribute>().Lang)
                .Select(l => $"builder:{l}");
            var runners = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(T =>
                    T.BaseType == typeof(ProgramRunner))
                .Select(T => T.GetCustomAttribute<LanguageAttribute>().Lang)
                .Select(l => $"runner:{l}");
            return runners.Concat(builders);
        }
        private async Task<IEnumerable<string>> CurrentImages()
        {
            var list = await dockerClient.Images.ListImagesAsync(new ImagesListParameters { All = true });
            return list.SelectMany(l => l.RepoTags);
        }

        private async Task<bool> CheckDockerConnection()
        {

            try
            {
                await dockerClient.System.PingAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cant connect to docker >>{ex.Message}");
                return false;
            }
            return true;
        }
        private static async Task CreateTarGZ(string tgzFilename, string sourceDirectory)
        {
            var outStream = File.Create(tgzFilename);
            var gzoStream = new GZipOutputStream(outStream);
            var tarOutputStream = new TarOutputStream(outStream);

            var filenames = Directory.GetFiles(sourceDirectory).Where(f => !f.EndsWith(".tar.gz"));
            foreach (var filename in filenames)
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
            // Note that the RootPath is currently case sensitive and must be forward slashes e.g. "c:/temp"
            // and must not end with a slash, otherwise cuts off first char of filename
            // This is scheduled for fix in next release
            //tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
            //if (tarArchive.RootPath.EndsWith("/"))
            //    tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);
            //if (char.IsUpper(tarArchive.RootPath[0]))
            //    tarArchive.RootPath = tarArchive.RootPath[0].ToString().ToLower() + tarArchive.RootPath.TrimStart(tarArchive.RootPath[0]);
            //AddDirectoryFilesToTar(tarArchive, sourceDirectory, false);

            //tarArchive.Close();
        }

        private static void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse)
        {
            // Optionally, write an entry for the directory itself.
            // Specify false for recursion here if we will add the directory's files individually.
            //TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory.);
            //tarArchive.WriteEntry(tarEntry, false);

            // Write each file to the tar.
            var filenames = Directory.GetFiles(sourceDirectory).Where(f => !f.EndsWith(".tar.gz"));
            foreach (var filename in filenames)
            {
                var tarEntry = TarEntry.CreateEntryFromFile(filename);
                //tarEntry.
                tarArchive.WriteEntry(tarEntry, true);
            }

            if (recurse)
            {
                string[] directories = Directory.GetDirectories(sourceDirectory);
                foreach (string directory in directories)
                    AddDirectoryFilesToTar(tarArchive, directory, recurse);
            }
        }
    }
}
