using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests
{
    public class UpdateAccountInfoRequest
    {
        [Required]
        [MinLength(1)]
        public string StudentId { get; set; }
        [Required]
        [MinLength(1)]
        public string FirstName { get; set; }
    }
}
