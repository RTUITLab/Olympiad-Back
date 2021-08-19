using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class GetMeResult : LoginResponse
    {
        public Dictionary<string, string[]> Claims { get; set; }
    }
}
