using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Executor.Executers.Build;
using Executor.Executers.Build.dotnet;
using Executor.Executers.Run;
using Executor.Executers.Run.dotnet;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Executor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ImagesBuilder();
            builder.CheckAndBuildImages();
            var options = JsonConvert.DeserializeObject<JObject>(await File.ReadAllTextAsync("appsettings.Secret.json"));
            var userName = options["username"].ToString();
            var password = options["password"].ToString();
            var address = options["apiAddress"].ToString();
            var dbManager = new DbManager(userName, password, address);
            var executor = new Executor(dbManager);
            executor.Start(CancellationToken.None);
            Console.ReadLine();
        }
    }
}
