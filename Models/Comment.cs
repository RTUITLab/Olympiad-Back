using System;

namespace Models;

public class Comment
{
    public Guid Id { get; set; }
    public string Raw { get; set; }
    public DateTimeOffset Time { get; set; }
    public Guid UserId { get; set; }
}
