using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Executor
{
    class DockerRunner
    {
        public void test()
        {
            var testDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            Console.WriteLine($"new dir is {testDir.FullName}");
            File.Copy(@"C:\Users\maksa\Desktop\test\Program.cs", Path.Combine(testDir.FullName, "Program.cs"));
            
            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"run --rm -v {testDir.FullName}:/home/src runner:dotnet"
                },
            };
            
            proccess.OutputDataReceived += Proccess_OutputDataReceived;
            proccess.ErrorDataReceived += Proccess_ErrorDataReceived;
            var success = proccess.Start();
            proccess.BeginErrorReadLine();
            proccess.BeginOutputReadLine();
            Console.WriteLine($"Started bool {success}");
            //proccess.StandardInput.WriteLine("echo lolka");
            //proccess.StandardInput.WriteLine("exit");
            proccess.WaitForExit();
            Console.WriteLine($"ENDED PROCESS");
            var publishDir = testDir.GetDirectories("publicated").FirstOrDefault();

            Directory.Move(publishDir.FullName, @"C:\New folder");
            testDir.Delete(true);
        }

        private void Proccess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"ERROR:: {e.Data}");
        }

        private void Proccess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"MESSAGE:: {e.Data}");
        }
    }
}
