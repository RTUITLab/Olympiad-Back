using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicAPI.Requests
{
    public class ListQueryParams
    {
        [Range(0, int.MaxValue)]
        public int Offset { get; set; }
        [Range(1, 200)]
        public int Limit { get; set; } = 50;
    }
}
