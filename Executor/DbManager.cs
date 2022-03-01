using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Executor.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Exercises;
using Olympiad.Shared.Models;
using Models.Solutions;
using PublicAPI.Requests;
using PublicAPI.Responses.Solutions;
using PublicAPI.Responses.ExercisesTestData;
using Olympiad.Shared;

namespace Executor
{
    class DbManager : ISolutionsBase
    {
        public const string DbManagerHttpClientName = nameof(DbManagerHttpClientName);

        private readonly IOptions<UserInfo> options;
        private readonly ILogger<DbManager> logger;
        private readonly HttpClient client;


        public DbManager(
            IOptions<UserInfo> options,
            IHttpClientFactory httpClientFactory,
            ILogger<DbManager> logger)
        {
            this.options = options;
            this.logger = logger;

            logger.LogInformation($"user name : {options.Value.UserName}");
            client = httpClientFactory.CreateClient(DbManagerHttpClientName);
        }
        public Task<ExerciseDataResponse[]> GetExerciseData(Guid exId)
        {
            return Invoke<ExerciseDataResponse[]>($"api/ExerciseData/all/{exId}");
        }

        public Task SaveChanges(Guid solutionId, SolutionStatus status)
            => InvokePost<object>($"api/Executor/{solutionId}/{(int)status}");


        public Task<List<Solution>> GetInQueueSolutions(string lang, int count)
        {
            return Invoke<List<Solution>>($"api/Executor?lang={lang}&count={count}");
        }

        public Task<Solution> GetSolutionInfo(Guid solutionId)
        {
            return Invoke<Solution>($"api/Executor?solutionId={solutionId}");
        }

        public Task<List<SolutionsStatisticResponse>> GetStatistic()
        {
            return Invoke<List<SolutionsStatisticResponse>>("api/Check/statistic");
        }

        public async Task SaveLog(Guid solutionId, Guid testDataId, SolutionCheckRequest solutionCheck)
        {
            if (solutionCheck.ProgramOut.Length > ExerciseDataLimitations.MAX_OUT_DATA_LENGTH)
            {
                solutionCheck.ProgramOut = solutionCheck.ProgramOut[..ExerciseDataLimitations.MAX_OUT_DATA_LENGTH];
            }
            var jsonString = JsonConvert.SerializeObject(solutionCheck);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var result = await InvokePostInternal($"api/executor/checklog/{solutionId}/{testDataId}", content);
            logger.LogDebug($"Sended logs, status code: {result.StatusCode}");
        }

        public async Task SaveBuildLog(Guid solutionId, BuildLogRequest buildLog)
        {
            var content = new StringContent(JsonConvert.SerializeObject(buildLog), Encoding.UTF8, "application/json");
            var result = await InvokePostInternal($"api/executor/buildlog/{solutionId}", content);
            logger.LogDebug($"Sended build logs, status code: {result.StatusCode}");
        }

        private async Task<T> Invoke<T>(string path)
        {
            try
            {
                var strResponse = await client.GetStringAsync(path);
                return JsonConvert.DeserializeObject<T>(strResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);
                logger.LogWarning($"cant invoke GET action with path >{path}<, try auth", ex);
                await Authorize();
                return await Invoke<T>(path);
            }
        }


        private async Task<T> InvokePost<T>(string path, HttpContent content = null)
        {
            var strResponse = await (await InvokePostInternal(path, content)).Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(strResponse);
        }

        private async Task<HttpResponseMessage> InvokePostInternal(string path, HttpContent content = null)
        {
            try
            {
                return await client.PostAsync(path, content);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"cant invoke POST action with path >{path}<, try auth", ex);
                await Authorize();
                return await InvokePostInternal(path, content);
            }

        }


        private async Task Authorize(int deep = 0)
        {
            var pack = new CredentialsRequest
            {
                Login = options.Value.UserName,
                Password = options.Value.Password
            };
            var content = JsonConvert.SerializeObject(pack);
            var body = new StringContent(content, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync("api/auth/login", body);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Can't connect to server, check credentials");
                }
                var strResponse = await response.Content.ReadAsStringAsync();
                logger.LogDebug($"server return {strResponse}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<JObject>(strResponse)["token"].ToString());
            }
            catch (Exception ex)
            {
                if (deep > 10)
                    throw;
                logger.LogWarning($"exception when auth #{deep}, wait for retry...", ex);
                await Task.Delay(TimeSpan.FromSeconds(5));
                await Authorize(deep + 1);
            }
        }
    }
}
