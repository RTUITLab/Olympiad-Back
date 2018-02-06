using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Responces
{
    public class LoginResponse
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string StudentId { get; set; }

        public int TotalScore { get; set; }
    }
}
