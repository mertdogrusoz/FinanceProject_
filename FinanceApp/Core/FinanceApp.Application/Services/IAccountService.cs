using FinanceApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{
	public interface IAccountService
	{
		Task<AccountDto> GetHesapByIdAsync(int id);
		Task<AccountDto> GetHesapByAccountNumberAsync(string AccountNumber);
		Task<AccountDto> CreateAccountAsync(AccountDto dto);
		Task<decimal> GetBalanceAsync(string AccountNumber);
		Task<IEnumerable<AccountDto>> GetAllAccountsAsync();


	}
}
