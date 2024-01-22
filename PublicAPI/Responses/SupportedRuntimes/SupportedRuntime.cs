namespace PublicAPI.Responses.SupportedRuntimes;

/// <summary>
/// Supported runtime
/// </summary>
/// <param name="WebKey">Unique key to use in API</param>
/// <param name="Title">Human readable title</param>
/// <param name="AcceptFileName">Filename for html input</param>
/// <param name="MarkdownDescription">Description of runtime for human in Markdown format</param>
public sealed record SupportedRuntime(string WebKey, string Title, string AcceptFileName, string MarkdownDescription);
