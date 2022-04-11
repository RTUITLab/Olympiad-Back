using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Pages.Results.Components;

public record DownloadAllSentFilesParameters(Guid ChallengeId, string ChallengeName, Func<Task<List<string>>> StudentIdsLoader);
