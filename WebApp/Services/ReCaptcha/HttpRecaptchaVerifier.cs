using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Models.Settings;

namespace WebApp.Services.ReCaptcha
{
    public class HttpRecaptchaVerifier : IRecaptchaVerifier
    {
        public const string HttpClientName = nameof(HttpRecaptchaVerifier) + nameof(HttpClientName);
        private readonly HttpClient httpClient;
        private readonly IOptions<RecaptchaSettings> options;

        public HttpRecaptchaVerifier(
            IHttpClientFactory httpClientFactory,
            IOptions<RecaptchaSettings> options)
        {
            this.httpClient = httpClientFactory.CreateClient(HttpClientName);
            this.options = options;
        }
        public async Task<RecaptchaResult> Check(string token, string ip)
        {
            var values = new Dictionary<string, string>
            {
                { "secret", options.Value.SecretKey },
                { "response", token },
                { "remoteip", ip }
            };

            var content = new FormUrlEncodedContent(values);
            try
            {
                var response = await httpClient.PostAsync("", content);
                var stringResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RecaptchaResult>(stringResponse);
            }
            catch (Exception ex)
            {
                throw StatusCodeException.BadRequest(ex.Message);
            }
        }
    }
}
