using Olympiad.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.HubModels
{
    public interface IHubClient
    {
        Task UpdateSolutionStatus(UpdateSolutionStatusModel statusModel);
    }
}
