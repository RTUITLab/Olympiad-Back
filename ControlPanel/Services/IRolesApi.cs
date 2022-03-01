using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services;

[Headers("Authorization: Bearer")]
public interface IRolesApi
{
    [Get("/api/roles/{userId}")]
    public Task<List<string>> GetRoles(Guid userid);
    [Post("/api/roles/{userId}/{roleName}")]
    public Task<List<string>> AddToRole(Guid userid, string roleName);
    [Delete("/api/roles/{userId}/{roleName}")]
    public Task<List<string>> RemoveFromRole(Guid userid, string roleName);
}
