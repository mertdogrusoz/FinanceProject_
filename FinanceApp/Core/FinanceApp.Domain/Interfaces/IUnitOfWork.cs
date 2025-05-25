using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Interfaces
{
	public interface IUnitOfWork:IDisposable
	{
		IAccountRepository AccountRepository { get; }
		IMoneyTransferRepository MoneyTransferRepository { get; }
		ILoanApplicationRepository LoanApplicationRepository { get; }
		Task<int> SaveChangesAsync();
		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollbackTransactionAsync();

	}
}
