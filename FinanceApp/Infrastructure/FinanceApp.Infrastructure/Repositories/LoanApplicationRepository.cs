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
	public class LoanApplicationRepository : ILoanApplicationRepository
	{

		private readonly AppDbContext _context;

		public LoanApplicationRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<LoanApplication> AddAsync(LoanApplication application)
		{
			_context.LoanApplications.Add(application);
			return application;
		}

		public async Task<IEnumerable<LoanApplication>> GetByAccountIdAsync(int accountId)
		{
			return await _context.LoanApplications
			   .Include(k => k.Account)
			   .Where(k => k.AccountId == accountId)
			   .OrderByDescending(k => k.ApplicationDate)
			   .ToListAsync();
		}

		public async Task<LoanApplication> GetByIdAsync(int id)
		{
			return await _context.LoanApplications
			 .Include(k => k.Account)
			 .FirstOrDefaultAsync(k => k.Id == id);
		}

		public async Task<IEnumerable<LoanApplication>> GetPendingApplicationsAsync()
		{
			return await _context.LoanApplications
				   .Include(k => k.Account)
				   .Where(k => k.Status == ApplicationStatus.Pending)
				   .OrderBy(k => k.ApplicationDate)
				   .ToListAsync();
		}

		public async Task UpdateAsync(LoanApplication application)
		{
			_context.LoanApplications.Update(application);
		}
	}
}
