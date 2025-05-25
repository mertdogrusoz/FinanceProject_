using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.DTOs
{
	public class LoanApplicationDto
	{
		public string AccountNumber { get; set; }
		public decimal RequestedAmount { get; set; }
		public int TermInMonths { get; set; }
		public LoanType LoanType { get; set; }
		public string Description { get; set; }

	}
}
