using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Responces
{
    public class CommentResponce
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Raw { get; set; }
        public DateTime Time { get; set; }
    }
}
