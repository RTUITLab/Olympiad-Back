using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Models;

namespace Executor.Executers.Build.PascalABC
{
    [Language("pasabc")]
    class PascalBuilder : ProgramBuilder
    {
        public PascalBuilder(Action proccessSolution, Action<DirectoryInfo, Solution> finishBuildSolution) : base(proccessSolution, finishBuildSolution)
        {
        }

        protected override DirectoryInfo Build(Solution solution)
        {
            var testDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            Console.WriteLine($"new dir is {testDir.FullName}");
            File.WriteAllText(Path.Combine(testDir.FullName, "Program.pas"), solution.Raw, Encoding.UTF8);

            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"run --rm -v {testDir.FullName}:/src builder:pasabc"
                },
            };
            SolutionStatus solStatus = SolutionStatus.InProcessing;
            proccess.OutputDataReceived += (D, E) => Proccess_OutputDataReceived(D, E, ref solStatus);
            proccess.ErrorDataReceived += (E, A) => Proccess_OutputDataReceived(E, A, ref solStatus);
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
            return testDir;
        }
        private void Proccess_OutputDataReceived(object sender, DataReceivedEventArgs e, ref SolutionStatus status)
        {
            if (e.Data?.Contains("Compile errors:") == true)
            {
                status = SolutionStatus.CompileError;
            }
        }
    }
}
