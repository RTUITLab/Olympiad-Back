using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Lessons
{
    public class CourseCompactResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int GroupsCount { get; set; }
    }
}
