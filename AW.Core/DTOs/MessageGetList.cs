using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Core.DTOs
{
	public class MessageGetList<T>
	{
		public int TotalCount { get; set; }
		public int TotalPage { get; set; }
		public object? DataSet { get; set; }
	}
}
