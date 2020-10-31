using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Lessons
{
    public class CourseCreateRequest
    {
        [Required]
        public string Title { get; set; }
    }
}
