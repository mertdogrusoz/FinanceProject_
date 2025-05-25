using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Interfaces
{
	public interface IAccountRepository
	{
		Task<Account> GetByIdAsync(int id);
		Task<Account> GetByAccountNumberAsync(string AccountNumber);
		Task<Account> GetByIBANAsync(string iban);
		Task<IEnumerable<Account>> GetAllAsync();
		Task<Account> AddAsync(Account account);
		Task UpdateAsync(Account account);
		Task DeleteAsync(int id);
		Task<bool> ExistsAsync(int id);
	}
}
