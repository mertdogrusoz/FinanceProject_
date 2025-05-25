using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Exceptions
{
	public class AccountNotFoundException:Exception
	{
		public AccountNotFoundException(string accountNumber)
			:base($"Hesap bulunamadı: {accountNumber}")
		{

			
		}
	}
}
