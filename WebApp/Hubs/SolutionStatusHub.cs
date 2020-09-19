using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models.HubModels;

namespace WebApp.Hubs
{

    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SolutionStatusHub : Hub<IHubClient>
    {
    }
}
