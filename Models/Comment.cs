using Models.Solutions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Raw { get; set; }
        public DateTimeOffset SendTime { get; set; }

        public Guid AuthorId { get; set; }
        public User Author { get; set; }

        public Guid SolutionId { get; set; }
        public Solution Solution { get; set; }
    }
}
