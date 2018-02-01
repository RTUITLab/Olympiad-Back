using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var options = JsonConvert.DeserializeObject<JObject>(await File.ReadAllTextAsync("appsettings.Secret.json"));
            var connectionString = options["ConnectionStrings"]["OlympDB"].ToString();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                Console.WriteLine("Enter");
                Console.WriteLine(db.Users.Count());
                Console.WriteLine("Exit");
            }



                Console.ReadLine();
        }
    }
}
