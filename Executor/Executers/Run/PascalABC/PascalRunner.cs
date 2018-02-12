using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Executor.Executers.Run.PascalABC
{
    [Language("pasabc")]
    class PascalRunner : ProgramRunner
    {
        public PascalRunner(Action proccessSolution) : base(proccessSolution)
        {
        }

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
                    Arguments = $"run --rm -v {binaries.FullName}:/src/ runner:pasabc"
                },
            };

            proccess.OutputDataReceived += (E, A) => Console.WriteLine("OUTPUT " + A.Data);
            proccess.ErrorDataReceived += (E, A) => Console.WriteLine("ERROR " + A.Data);
            var success = proccess.Start();
            proccess.BeginErrorReadLine();
            proccess.BeginOutputReadLine();
            Console.WriteLine($"Started bool {success}");
            proccess.WaitForExit();

            var errorData = File.ReadAllText(Path.Combine(binaries.FullName, "err.txt")).TrimEnd();
            if (errorData != string.Empty)
                return SolutionStatus.RunTimeError;

            var outData = File.ReadAllText(Path.Combine(binaries.FullName, "out.txt")).TrimEnd();
            if (outData == testData.OutData.TrimEnd())
                return SolutionStatus.Sucessful;

            return SolutionStatus.WrongAnswer;
        }
    }
}
