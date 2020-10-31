using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public class GroupsService
    {
        private const string available = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Random random = new Random();
        public string GenerateInvoteToken()
        {
            return string.Join("", Enumerable.Repeat(available, 20).Select(a => a[random.Next(a.Length)]));
        }
    }
}
