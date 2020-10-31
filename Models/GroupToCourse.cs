using Models.Lessons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class GroupToCourse
    {
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
