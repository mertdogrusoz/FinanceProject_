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
	}
}
