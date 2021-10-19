using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests.Account
{
    public class GenerateUserRequest
    {
        [Required]
        [MinLength(1)]
        public string ID { get; set; }
        [Required]
        [MinLength(1)]
        public string Name {  get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        public List<ClaimRequest> Claims {  get; set; }
    }
}
