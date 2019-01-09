using System;

namespace PublicAPI.Responses
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
