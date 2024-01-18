using System;

namespace PublicAPI.Responses.Users;

public class UserInfoResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string Email { get; set; }

    public string StudentId { get; set; }
}
