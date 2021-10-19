using Models;
using OneOf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Services.Authorization
{
    public interface IUserAuthorizationService
    {
        Task<OneOf<(string accessToken, User user), GetJwtForUserError>> GetJwtTokenForUser(Guid userId);
        Task<OneOf<(string accessToken, User user), GetJwtForUserError>> GetJwtTokenForUser(string login, string password, bool skipPasswordCheck = false);

        public enum GetJwtForUserError
        {
            IncorrectCredentials
        }
    }
}
