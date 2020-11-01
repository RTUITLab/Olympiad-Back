using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Requests
{
    public class PostComment
    {
        public Guid SolutionId { get; set; }
        public string Raw { get; set; }
        public int RowNumber { get; set; }
    }
}
