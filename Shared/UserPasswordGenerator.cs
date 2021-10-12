using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Olympiad.Shared
{
    public class UserPasswordGenerator
    {
        private const int MIN = 11111111;
        private const int MAX = 99999999;
        private readonly Random random = new Random();

        public string GeneratePassword()
        {
            return random.Next(MIN, MAX).ToString();
        }
    }
}
