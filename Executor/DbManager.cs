using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Executor.Logging;

namespace Executor
{
    class DbManager
    {
        private readonly HttpClient client;
        private readonly string userName;
        private readonly string password;
        private string accessToken;
        private Logger<DbManager> logger;

        public DbManager(string userName, string password, string remoteAdress)
        {
            logger = Logger<DbManager>.CreateLogger(remoteAdress);
            logger.LogInformation($"user name : {userName}");
            logger.LogInformation($"password : {password}");
            logger.LogInformation($"address : {remoteAdress}");
            client = new HttpClient()
            {
                BaseAddress = new Uri(remoteAdress)
            };
            this.userName = userName;
            this.password = password;
            Authorize();
        }
        public ExerciseData[] GetExerciseData(Guid exId)
        {
            return Invoke<ExerciseData[]>($"api/ExerciseData/{exId}");
        }

        public void SaveChanges(Guid solutionId, SolutionStatus status)
        {
            InvokePost<object>($"api/Executor/{solutionId}/{(int)status}");
        }

        public List<Solution> GetInQueueSolutions()
        {
            return Invoke<List<Solution>>("api/Executor");
        }


        private T Invoke<T>(string path)
        {
            try
            {
                var strResponse = client.GetStringAsync(path).Result;
                return JsonConvert.DeserializeObject<T>(strResponse);
            } catch (Exception ex)
            {
                logger.LogWarning($"cant invoke GET action with path >{path}<, try auth", ex);
                Authorize();
                return Invoke<T>(path);
            }
        }

        private T InvokePost<T>(string path)
        {
            try
            {
                var strResponse = client.PostAsync(path, null).Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(strResponse);
            } catch (Exception ex)
            {
                logger.LogWarning($"cant invoke POST action with path >{path}<, try auth", ex);
                Authorize();
                return InvokePost<T>(path);
            }
            
        }


        private void Authorize()
        {
            var pack = new
            {
                UserName = userName,
                Password = password
            };
            var content = JsonConvert.SerializeObject(pack);
            logger.LogDebug($"auth with data {content}");
            var body = new StringContent(content, Encoding.UTF8, "application/json");
            string strResponse = "";
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
