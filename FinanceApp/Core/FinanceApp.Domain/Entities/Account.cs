using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Entities
{
	public class Account
	{
		public int Id { get; set; }
		public string AccountNumber { get; set; }
		public string IBAN { get; set; }
		public decimal BalanceAmount { get; set; }
		public string AccountOwner { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsActive { get; set; }

	}

}
