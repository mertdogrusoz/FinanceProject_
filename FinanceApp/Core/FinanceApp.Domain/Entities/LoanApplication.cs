using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Entities
{
	public class LoanApplication
	{
		public int Id { get; set; }
		public int AccountId { get; set; }
		public decimal RequestedAmount { get; set; }
		public int TermDuration { get; set; }
		public LoanType LoanType { get; set; }
		public ApplicationStatus Status { get; set; }
		public DateTime ApplicationDate { get; set; }
		public DateTime? EvaluationDate { get; set; }
		public string Description { get; set; }

		public virtual Account Account { get; set; }
	}
	public enum ApplicationStatus
	{
		Pending = 0,
		Approved = 1,
		Rejected = 2,
		Cancelled = 3
	}
	public enum LoanType
	{
		Personal = 0,
		Housing = 1,
		Vehicle = 2,
		Commercial = 3
	}
}
