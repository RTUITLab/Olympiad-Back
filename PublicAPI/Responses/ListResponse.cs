using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses
{
	public class ListResponse<T>
	{
		public List<T> Data { get; set; }
		public int Offset { get; set; }
		public int Limit { get; set; }
		public int Total { get; set; }
	}
}
