using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.Lessons
{
    public class CourseResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<GroupCompactResponse> Groups { get; set; }
    }
}
