using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Raw { get; set; }
        public DateTime Time { get; set; }
        public Guid UserId { get; set; }
    }
}
