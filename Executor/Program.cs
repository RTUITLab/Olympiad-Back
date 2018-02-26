using System;
using System.Collections;
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
        const string settingsFileName = "appsettings.Secret.json";
        static JObject fileContent;
        static string address;
        static string userName;
        static string password;

        static async Task Main(string[] args)
        {
            if (!SetupConfigs(args))
                return;
            var builder = new ImagesBuilder();
            if (!builder.CheckAndBuildImages())
            {
                Console.WriteLine("host must have docker!");
                return;
            };
            var dbManager = new DbManager(userName, password, address);
            var executor = new Executor(dbManager);
            executor.Start(CancellationToken.None);
            Console.ReadLine();
        }

        static bool SetupConfigs(string[] args)
        {
            if (args.Contains("-help"))
            {
                WriteHelp();
                return false;
            }
            try
            {
                password = GetParameter(args, "-password") ?? GetParameter("password") ?? Throw($"Can't get info for password");
                userName = GetParameter(args, "-username") ?? GetParameter("username") ?? Throw($"Can't get info for username");
                address = GetParameter(args, "-host") ?? GetParameter("host") ?? Throw($"Can't get info for apiadress");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}, use -help for get help");
                return false;
            }
            return true;
        }


        static string GetParameter(string[] args, string paramKey)
            => args.SkipWhile(s => s != paramKey).Skip(1).FirstOrDefault();

        static string GetParameter(string paramKey)
            => /*fileContent?[paramKey]?.ToString() ?? */
                (File.Exists(settingsFileName) ?
                    (fileContent = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(settingsFileName) ?? "{}"))?[paramKey]?.ToString()
                    : null);
        static string Throw(string message) => throw new Exception(message);

        static void WriteHelp()
        {
            System.Console.WriteLine($"Use with parameters if need, or write in {settingsFileName} file");
            System.Console.WriteLine($"parameters use first, file have low propoty");
            System.Console.WriteLine("---------------");
            System.Console.WriteLine("terminal parameters");
            System.Console.WriteLine("host for specific host");
            System.Console.WriteLine("username for specific username");
            System.Console.WriteLine("password for specific password");

            System.Console.WriteLine("---------------");
            System.Console.WriteLine("json file parameters");
            System.Console.WriteLine("-host for specific host");
            System.Console.WriteLine("-username for specific username");
            System.Console.WriteLine("-password for specific password");
        }
    }
}
