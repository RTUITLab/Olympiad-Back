using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
    public class ListResponseWithMatch<T> : ListResponse<T>
    {
        public string Match { get; set; }
    }
}
