using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.DTOs
{
	public class CreateAccountDto
	{
		public string AccountOwner { get; set; }
		public decimal FirstAmount { get; set; }
	}
}
