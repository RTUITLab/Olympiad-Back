using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Olympiad.Shared
{
    public static class DefaultClaims
    {
        public static class NeedResetPassword
        {
            public const string Type = "reset_password";
            public const string Value = "need";
            public static Claim Claim => new Claim(Type, Value);
        }

        public static class AddToChallenge
        {
            public const string Type = "AddToChallenge";
        }
    }
}
