using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public static class IHubClientsExtensions
    {
        public static T User<T>(this IHubClients<T> hubClients, Guid userId)
        {
            return hubClients.User(userId.ToString());
        }
    }
}
