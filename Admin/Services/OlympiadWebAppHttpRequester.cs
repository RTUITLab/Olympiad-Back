using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Olympiad.Admin.Services
{
    public class OlympiadWebAppHttpRequester
    {
        public const string HttpClientName = nameof(OlympiadWebAppHttpRequester) + nameof(HttpClientName);
        private readonly HttpClient httpClient;

        public OlympiadWebAppHttpRequester(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient(HttpClientName);
        }
        public async Task<int> ForceResetQueue()
        {
            var response =  await httpClient.PostAsync("/api/admin/forceresetqueue", null);
            var content = await response.Content.ReadAsStringAsync();
            return int.Parse(content);
        }
    }
}
