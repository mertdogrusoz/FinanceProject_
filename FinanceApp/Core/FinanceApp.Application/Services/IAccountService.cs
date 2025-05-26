using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{
	public interface IAccountService
	{
		Task<AccountDto> GetAccountByIdAsync(int id);
		Task<AccountDto> GetAccountByAccountNumberAsync(string AccountNumber);
		Task<AccountDto> CreateAccountAsync(CreateAccountDto dto);
		Task<decimal> GetBalanceAsync(string AccountNumber);
		Task<IEnumerable<AccountDto>> GetAllAccountsAsync();


	}
}
