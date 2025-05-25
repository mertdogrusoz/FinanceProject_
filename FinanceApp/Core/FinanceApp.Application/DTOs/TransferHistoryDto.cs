using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.DTOs
{
	public class TransferHistoryDto
	{
		public int Id { get; set; }
		public string SenderAccountNumber { get; set; }
		public string ReceiverAccountNumber { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public DateTime TransferDate { get; set; }
		public string ReferenceNumber { get; set; }
		public string Status { get; set; }
	}
}
