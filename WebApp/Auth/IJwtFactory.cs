using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Auth
{
    public interface IJwtFactory
    {
        string GenerateToken(User user, IList<string> roles);
    }
}
