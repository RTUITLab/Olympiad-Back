using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace Executor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            new DockerRunner().test();
            Console.WriteLine("Program end");
            Console.ReadLine();
        }
    }
}
