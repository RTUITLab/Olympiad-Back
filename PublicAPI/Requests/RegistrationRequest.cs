using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicAPI.Requests
{
    public class CreateUserDataModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentID { get; set; }
    }
    public class RegistrationRequest : CreateUserDataModel
    {
        public string RecaptchaToken { get; set; }
    }
}
