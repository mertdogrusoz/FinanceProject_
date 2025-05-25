using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Exceptions
{
	public class InsufficientBalanceException:Exception
	{
		public InsufficientBalanceException(string accountNumber, decimal BalanceAmount, decimal RequestedAmount)
			: base($"Hesap {accountNumber} için yetersiz bakiye. Mevcut: {BalanceAmount:C}, İstenen: {RequestedAmount:C}")
		{
			
		}
	}
}
