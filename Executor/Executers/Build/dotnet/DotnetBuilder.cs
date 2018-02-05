using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Executor.Executers.Build.dotnet
{
    class DotnetBuilder : ProgramBuilder
    {
        public DotnetBuilder(Action proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution)
            : base(proccessSolution, finishBuildSolution)
        {
        }

        protected override string Lang => "csharp";

        protected override DirectoryInfo Build(Solution solution)
        {
            var testDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            Console.WriteLine($"new dir is {testDir.FullName}");
            File.WriteAllText(Path.Combine(testDir.FullName, "Program.cs"), solution.Raw, Encoding.UTF8);
            

            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"run --rm -v {testDir.FullName}:/home/src builder:dotnet"
                },
            };

            proccess.OutputDataReceived += (E, A) => Console.WriteLine("OUTPUT " + A.Data);
            proccess.ErrorDataReceived += (E, A) => Console.WriteLine("ERROR " + A.Data);
            var success = proccess.Start();
            proccess.BeginErrorReadLine();
            proccess.BeginOutputReadLine();
            Console.WriteLine($"Started bool {success}");
            proccess.WaitForExit();
            Console.WriteLine($"ENDED PROCESS");
            var publishDir = testDir.GetDirectories("publicated").FirstOrDefault();
            var nextFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.Move(publishDir.FullName, nextFolderPath);
            testDir.Delete(true);
            return new DirectoryInfo(nextFolderPath);
        }
    }

}

