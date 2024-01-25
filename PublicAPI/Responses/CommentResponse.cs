using System;

namespace PublicAPI.Responses;

public class CommentResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Raw { get; set; }
    public DateTimeOffset Time { get; set; }
}
