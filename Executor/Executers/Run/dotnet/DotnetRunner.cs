using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Models;

namespace Executor.Executers.Run.dotnet
{
    class DotnetRunner : ProgramRunner
    {
        public DotnetRunner(Action proccessSolution) : base(proccessSolution)
        {
        }

        protected override string Lang => "csharp";

        protected override SolutionStatus Run(DirectoryInfo binaries, ExerciseData testData)
        {
            File.WriteAllText(Path.Combine(binaries.FullName, "in.txt"), testData.InData);
            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"run --rm -v {binaries.FullName}:/home runner:dotnet"
                },
            };

            proccess.OutputDataReceived += (E, A) => Console.WriteLine("OUTPUT " + A.Data);
            proccess.ErrorDataReceived += (E, A) => Console.WriteLine("ERROR " + A.Data);
            var success = proccess.Start();
            proccess.BeginErrorReadLine();
            proccess.BeginOutputReadLine();
            Console.WriteLine($"Started bool {success}");
            proccess.WaitForExit();
            var outData = File.ReadAllText(Path.Combine(binaries.FullName, "out.txt")).TrimEnd();
            Console.WriteLine(outData);
            if (outData == testData.OutData.TrimEnd())
                return SolutionStatus.Sucessful;
            return SolutionStatus.WrongAnswer;
        }
    }
}
