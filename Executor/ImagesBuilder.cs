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
        public bool CheckAndBuildImages()
        {
            if (!CheckDockerOnMachine()) return false;
            var needToBuild = NeedImages().Except(CurrentImages()).ToList();
            needToBuild.ForEach(Console.WriteLine);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Executers");
            var folerPairs = needToBuild
                .Select(N => (N, folder: N.StartsWith("runner") ? "Run" : "Build"))
                .Select(P => (P.N, folder: Path.Combine(path, P.folder, P.N.Split(":")[1], "DockerFile"	)))
                .ToList();
            folerPairs.ForEach(P =>
            {
                Console.WriteLine(P.N);
                Console.WriteLine(P.folder);
                BuildImage(P.N, P.folder);
                Console.WriteLine(new string('-', 10));
            });
            return true;
        }

        private void BuildImage(string imageName, string dockerFilePath)
        {
            if (!File.Exists(dockerFilePath) && false)
            {
                Console.WriteLine($"file {dockerFilePath} not exists");
                return;
            }
            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"build -t {imageName} -f {dockerFilePath} ."
                },
            };
            proccess.OutputDataReceived += (A, B) => Console.WriteLine("OUT " + B.Data);
            proccess.ErrorDataReceived += (A, B) => Console.WriteLine("ERR " + B.Data);
            proccess.Start();
            proccess.BeginOutputReadLine();
            proccess.BeginErrorReadLine();
            proccess.WaitForExit();
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
            return runners.Concat(builders).ToList();
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

        private bool CheckDockerOnMachine()
        {
            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = "--help"
                },
            };
            var error = false;
            proccess.ErrorDataReceived += (E, D) => 
		{
			Console.WriteLine("ERROR" + D.Data ?? "NULL");
			error |= D.Data != null;
		};
            proccess.OutputDataReceived += (E, D) => {
			Console.WriteLine("OUT" + D.Data);
		};
            try
            {
                proccess.Start();
                proccess.BeginErrorReadLine();
                proccess.BeginOutputReadLine();
                proccess.WaitForExit();
            }
            catch (System.Exception)
            {
                return false;
            }
            return !error;
        }
    }
}
