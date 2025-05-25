using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Interfaces;
using FinanceApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Infrastructure.Repositories
{
	public class AccountRepository : IAccountRepository
	{
		private readonly AppDbContext _context;

		public AccountRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Account> AddAsync(Account account)
		{
			_context.Accounts.Add(account);
			return account;
		}

		public async Task DeleteAsync(int id)
		{
			var account = await GetByIdAsync(id);
			if (account != null)
			{
				account.IsActive = false;
				_context.Accounts.Update(account);
			}
		}

		public async Task<bool> ExistsAsync(int id)
		{ 
			return await _context.Accounts.AnyAsync(h => h.Id == id);
		}

		public async Task<IEnumerable<Account>> GetAllAsync()
		{
			return await _context.Accounts.Where(h => h.IsActive).ToListAsync();
		}

		public async Task<Account> GetByAccountNumberAsync(string AccountNumber)
		{
			return await _context.Accounts.FirstOrDefaultAsync(h => h.AccountNumber == AccountNumber);
		}

		public async Task<Account> GetByIBANAsync(string iban)
		{
			return await _context.Accounts.FirstOrDefaultAsync(h => h.IBAN == iban);
		}

		public async Task<Account> GetByIdAsync(int id)
		{
			return await _context.Accounts.FindAsync(id);
		}

		public async Task UpdateAsync(Account account)
		{
			_context.Accounts.Update(account);
		}
	}
}
