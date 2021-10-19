using Microsoft.Extensions.Logging;
using Olympiad.ControlPanel.Pages.Users.GenerateUsers;
using PublicAPI.Requests.Account;
using PublicAPI.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services
{
    internal class GenerateUserService
    {
        private readonly ILogger<GenerateUserService> logger;
        private readonly IControlPanelApiService apiService;

        public GenerateUserService(
            ILogger<GenerateUserService> logger,
            IControlPanelApiService apiService)
        {
            this.logger = logger;
            this.apiService = apiService;
        }
        public async Task<UserGenerateResult> GenerateUser(UserGenerateRow userGenerateRow)
        {
            try
            {
                await apiService.GenerateNewUser(new GenerateUserRequest
                {
                    ID = userGenerateRow.StudentID,
                    Name = userGenerateRow.FirstName,
                    Password = userGenerateRow.Password,
                    Claims = userGenerateRow.Claims
                        .Where(c => !string.IsNullOrEmpty(c.Value))
                        .Select(c => new ClaimRequest { Type = c.Type, Value = c.Value }).ToList()
                });
                return new UserGenerateResult(userGenerateRow, true, default);
            } 
            catch (Refit.ApiException apiEx) when(apiEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorMessage = "Некорректные данные пользователя";
                if (apiEx.Content?.Contains("DuplicateUserName") == true)
                {
                    errorMessage = "ID польователя уже занято";
                }
                else if(apiEx.Content?.Contains("Password") == true)
                {
                    errorMessage = "Пароль не удовлетворяет требованиям системы";
                }
                return new UserGenerateResult(userGenerateRow, false, errorMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "unexpected error with user creating");
                return new UserGenerateResult(userGenerateRow, false, "Непредвиденная ошибка при создании пользователя");
            }
        }
    }
}
