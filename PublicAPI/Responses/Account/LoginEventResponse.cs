using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Account
{
    public class LoginEventResponse
    {
        public DateTimeOffset Date { get; set; }
        public string UserAgent { get; set; }
        public string IP { get; set; }
    }
}
