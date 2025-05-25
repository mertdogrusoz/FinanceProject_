using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Entities
{
	public class CreateAccountDto
	{
		public string AccountOwner { get; set; }
		public decimal FirstAmount { get; set; }
	}
}
