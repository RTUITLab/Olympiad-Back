using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using PublicAPI.Responses.Challenges.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Services;

public class ChallengeTotalReportCreator
{
    private readonly IJSRuntime js;
    private readonly IChallengesApi challengesApi;
    private readonly IExercisesApi exercisesApi;
    private readonly ILogger<ChallengeTotalReportCreator> logger;
    private const int pageSize = 100;
    public bool IsCreating { get; private set; }
    public ChallengeTotalReportCreator(
        IJSRuntime js, 
        IChallengesApi challengesApi, 
        IExercisesApi exercisesApi,
        ILogger<ChallengeTotalReportCreator> logger)
    {
        this.js = js;
        this.challengesApi = challengesApi;
        this.exercisesApi = exercisesApi;
        this.logger = logger;
    }

    public async Task CreateReport(Guid challengeId, string challengeName, string? match)
    {
        IsCreating = true;
        try
        {
            var excelGeneratingModule = await js.InvokeAsync<IJSObjectReference>("import", "./js/excelReportsCreating.js");
            var data = await LoadAllData(challengeId, match);
            var exercises = await exercisesApi.GetExercisesAsync(challengeId);
            await excelGeneratingModule.InvokeVoidAsync("createReport", challengeName, data, exercises);
        }
        finally
        {
            IsCreating = false;
        }
    }

    private async Task<List<UserChallengeResultsResponse>> LoadAllData(Guid challengeId, string? match)
    {
        int offset = 0;
        int total;
        // prevent dublicates while multiple requests
        var alldata = new Dictionary<Guid, UserChallengeResultsResponse>();
        do
        {
            var response = await challengesApi.GetUserResultsForChallenge(challengeId, match, offset, pageSize);
            total = response.Total;
            offset += response.Data.Count;
            foreach (var item in response.Data)
            {
                alldata[item.User.Id] = item;
            }
        } while (offset < total);

        return alldata.Values.OrderBy(v => v.User.StudentId).ToList();
    }
}
