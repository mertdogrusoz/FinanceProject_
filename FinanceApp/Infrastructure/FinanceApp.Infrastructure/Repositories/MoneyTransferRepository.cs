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
	public class MoneyTransferRepository : IMoneyTransferRepository
	{
		private readonly AppDbContext _context;

		public MoneyTransferRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Moneytransfer> AddAsync(Moneytransfer transfer)
		{
			_context.Moneytransfers.Add(transfer);
			return transfer;
		}

		public async Task<IEnumerable<Moneytransfer>> GetByAccountIdAsync(int accountId)
		{
			return await _context.Moneytransfers
			  .Include(t => t.SenderAccount)
			  .Include(t => t.ReceiverAccount)
			  .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId)
			  .OrderByDescending(t => t.TransferDate)
			  .ToListAsync();

		}

		public async Task<Moneytransfer> GetByIdAsync(int id)
		{
			return await _context.Moneytransfers
			  .Include(t => t.SenderAccount)
			  .Include(t => t.ReceiverAccount)
			  .FirstOrDefaultAsync(t => t.Id == id);

		}

		public async Task<Moneytransfer> GetByReferansAsync(string referenceNumber)
		{
			return await _context.Moneytransfers
			   .Include(t => t.SenderAccount)
			   .Include(t => t.ReceiverAccount)
			   .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber);

		}

		public async Task<IEnumerable<Moneytransfer>> GetTransferHistoryAsync(int accountId, DateTime? started = null, DateTime? finished = null)
		{
			var query = _context.Moneytransfers
		  .Include(t => t.SenderAccount)    
		  .Include(t => t.ReceiverAccount) 
		  .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId)
		  .Where(t => t.TransferDate >= started && t.TransferDate <= finished);

			return await query.ToListAsync();
		}

		public async Task UpdateAsync(Moneytransfer transfer)
		{
			_context.Moneytransfers.Update(transfer);
		}
	}
}
