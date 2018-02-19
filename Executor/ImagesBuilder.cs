﻿using Executor.Executers.Build;
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
            CurrentImages().ForEach(Console.WriteLine);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Executers");
            Directory.GetDirectories(path).ToList().ForEach(Console.WriteLine);
        }

        private List<string> NeedImages()
        {
            var executeWorkers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(T =>
                    T.BaseType == typeof(ProgramBuilder) ||
                    T.BaseType == typeof(ProgramRunner))
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
