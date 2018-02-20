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

namespace Executor
{
    class ImagesBuilder
    {
        public void CheckAndBuildImages()
        {
            var needToBuild = NeedImages().Except(CurrentImages()).ToList();
            needToBuild.ForEach(Console.WriteLine);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Executers");
            var folerPairs = needToBuild
                .Select(N => (N, folder: N.StartsWith("runner") ? "Run" : "Build"))
                .Select(P => (P.N, folder: Path.Combine(path, P.folder)))
                .ToList();
            folerPairs.ForEach(P =>
            {
                Console.WriteLine(P.N);
                Console.WriteLine(P.folder);
                var dockerFile = Path.Combine(P.folder, "DockerFile");
                if (File.Exists(dockerFile))
                    Console.WriteLine(File.ReadAllText(dockerFile));
                Console.WriteLine(new string('-', 10));
            });
            Directory.GetDirectories(path).ToList().ForEach(Console.WriteLine);
        }

        private List<string> NeedImages()
        {
            var builders = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(T =>
                    T.BaseType == typeof(ProgramBuilder))
                .Select(T => T.GetCustomAttribute<LanguageAttribute>().Lang)
                .Select(L => $"builder:{L}");
            var runners = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(T =>
                    T.BaseType == typeof(ProgramRunner))
                .Select(T => T.GetCustomAttribute<LanguageAttribute>().Lang)
                .Select(L => $"runner:{L}");
            var l = runners.Concat(builders).ToList();
            return l;
        }
        private List<string> CurrentImages()
        {
            var list = new List<string>();
            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"images"
                },
            };
            proccess.OutputDataReceived += (E, D) =>
            {
                if (D.Data != null)
                {
                    var data = D.Data.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToList();
                    list.Add($"{data[0]}:{data[1]}");

                }
            };
            proccess.Start();
            proccess.BeginOutputReadLine();
            proccess.WaitForExit();
            return list;
        }
    }
}
