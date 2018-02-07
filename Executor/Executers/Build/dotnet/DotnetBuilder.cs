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
    [Language("csharp")]
    class DotnetBuilder : ProgramBuilder
    {
        public DotnetBuilder(Action proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution)
            : base(proccessSolution, finishBuildSolution)
        {
        }

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
            SolutionStatus solStatus = SolutionStatus.InProcessing;
            proccess.OutputDataReceived += (D, E) => Proccess_OutputDataReceived(D, E, ref solStatus);
            proccess.ErrorDataReceived += (E, A) => Console.WriteLine("ERROR " + A.Data);
            var success = proccess.Start();
            proccess.BeginErrorReadLine();
            proccess.BeginOutputReadLine();
            Console.WriteLine($"Started bool {success}");
            proccess.WaitForExit();
            Console.WriteLine($"ENDED PROCESS");
            if (solStatus == SolutionStatus.CompileError)
            {
                solution.Status = SolutionStatus.CompileError;
                return null;
            }
            var publishDir = testDir.GetDirectories("publicated").FirstOrDefault();
            var nextFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.Move(publishDir.FullName, nextFolderPath);
            testDir.Delete(true);
            return new DirectoryInfo(nextFolderPath);
        }

        private void Proccess_OutputDataReceived(object sender, DataReceivedEventArgs e, ref SolutionStatus status)
        {
            if (e.Data?.Contains("Build FAILED") == true)
            {
                status = SolutionStatus.CompileError;
            }
        }
    }

}

