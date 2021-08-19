
using PublicAPI.Responses.Solutions;
using System.Net.Http.Json;

namespace Olympiad.ControlPanel.Services;
public class ControlPanelApiService
{
    private readonly HttpClient httpClient;

    public ControlPanelApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<SolutionsStatisticResponse>> GetSolutionsStatisticsAsync()
	{
		var data = await httpClient.GetFromJsonAsync<List<SolutionsStatisticResponse>>("api/check/statistic");
        return data ?? new List<SolutionsStatisticResponse>();
    }
}
