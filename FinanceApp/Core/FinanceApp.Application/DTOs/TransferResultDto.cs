using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.DTOs
{
	public class TransferResultDto
	{
		public bool IsSuccessful { get; set; }
		public string ReferenceNumber { get; set; }
		public string Message { get; set; }
		public DateTime TransferDate { get; set; }
	}
}
