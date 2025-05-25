using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Interfaces
{
	public interface ILoanApplicationRepository
	{
		Task<LoanApplication> GetByIdAsync(int id);
		Task<IEnumerable<LoanApplication>> GetByAccountIdAsync(int accountId);
		Task<LoanApplication> AddAsync(LoanApplication application);
		Task UpdateAsync(LoanApplication application);
		Task<IEnumerable<LoanApplication>> GetPendingApplicationsAsync();

	}
}
