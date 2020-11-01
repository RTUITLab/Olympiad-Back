using PublicAPI.Responses.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class CommentResponse
    {
        public string Raw { get; set; }
        public int RowNumber { get; set; }
        public DateTimeOffset SendTime { get; set; }

        public Guid AuthorId { get; set; }
        public UserInfoResponse Author { get; set; }
    }
}
