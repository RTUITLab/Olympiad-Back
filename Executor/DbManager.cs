using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Executor.Logging;
using Executor.Models.Settings;
using Microsoft.Extensions.Options;
using Models.Exercises;
using Shared.Models;
using Models.Solutions;

namespace Executor
{
    class DbManager
    {
        public const string DbManagerHttpClientName = nameof(DbManagerHttpClientName);

        private readonly IOptions<UserInfo> options;
        private readonly HttpClient client;
        private readonly Logger<DbManager> logger;

        public DbManager(
            IOptions<UserInfo> options,
            IHttpClientFactory httpClientFactory)
        {
            this.options = options;
            logger = Logger<DbManager>.CreateLogger();
            logger.LogInformation($"user name : {options.Value.UserName}");
            client = httpClientFactory.CreateClient(DbManagerHttpClientName);
            Authorize();
        }
        public Task<ExerciseData[]> GetExerciseData(Guid exId)
        {
            return Invoke<ExerciseData[]>($"api/ExerciseData/all/{exId}");
        }

        public Task SaveChanges(Guid solutionId, SolutionStatus status)
            => InvokePost<object>($"api/Executor/{solutionId}/{(int)status}");


        public Task<List<Solution>> GetInQueueSolutions()
        {
            return Invoke<List<Solution>>("api/Executor");
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
                logger.LogWarning($"cant invoke GET action with path >{path}<, try auth", ex);
                Authorize();
                return await Invoke<T>(path);
            }
        }

        private async Task<T> InvokePost<T>(string path)
        {
            try
            {
                var strResponse = await client.PostAsync(path, null).Result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(strResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"cant invoke POST action with path >{path}<, try auth", ex);
                Authorize();
                return await InvokePost<T>(path);
            }

        }


        private void Authorize()
        {
            var pack = new
            {
                options.Value.UserName,
                options.Value.Password
            };
            var content = JsonConvert.SerializeObject(pack);
            var body = new StringContent(content, Encoding.UTF8, "application/json");
            string strResponse;
            try
            {
                strResponse = client.PostAsync("api/auth/login", body).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                logger.LogWarning($"exception when auth", ex);
                throw;
            }
            logger.LogDebug($"server return {strResponse}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<JObject>(strResponse)["Token"].ToString());
        }
    }
}
