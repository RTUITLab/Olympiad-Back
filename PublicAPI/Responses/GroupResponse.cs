using PublicAPI.Responses.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class GroupResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string InviteToken { get; set; }
        public string LessonsTime { get; set; }
        public List<UserInfoResponse> Users { get; set; }
    }
}
