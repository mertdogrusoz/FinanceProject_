using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Events
{
	public class MoneyTransferedEvent
	{
		public string AccountNumber { get; }
		public decimal Amount { get; }
		public bool IsCredit { get; }
		public DateTime OccurredAt { get; }
		public string RecipientEmail { get; set; }

		public MoneyTransferedEvent(string accountNumber, decimal amount, bool isCredit,string recipientEmail)
		{
			AccountNumber = accountNumber;
			Amount = amount;
			IsCredit = isCredit;
			OccurredAt = DateTime.UtcNow;
			RecipientEmail = recipientEmail;
		}
	}
	
}
