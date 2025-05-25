using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Interfaces
{
	public interface IMoneyTransferRepository
	{
		Task<Moneytransfer> GetByIdAsync(int id);
		Task<Moneytransfer> GetByReferansAsync(string referenceNumber);
		Task<IEnumerable<Moneytransfer>> GetByAccountIdAsync(int accountId);
		Task<Moneytransfer> AddAsync(Moneytransfer transfer);
		Task UpdateAsync(Moneytransfer transfer);
		Task<IEnumerable<Moneytransfer>> GetTransferHistoryAsync(int accountId, DateTime? started = null, DateTime? finished = null);
	}
}
