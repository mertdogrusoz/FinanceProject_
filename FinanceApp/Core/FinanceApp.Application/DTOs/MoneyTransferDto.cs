using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.DTOs
{
	public class MoneyTransferDto
	{
		public string SenderAccountId { get; set; }
		public string ReceiverAccountId { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public int Id { get; set; }
		public DateTime TransferDate { get; set; }
		public string SenderAccountNumber { get; set; }
		public string SenderAccountName { get; set; }
		public string ReceiverAccountNumber { get; set; }
		public string ReceiverAccountName { get; set; }
		public string TransferType { get; set; } // "Giden" veya "Gelen"
	}
}
