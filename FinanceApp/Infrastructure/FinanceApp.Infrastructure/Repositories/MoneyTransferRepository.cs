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

		public Task<IEnumerable<Moneytransfer>> GetByAccountIdAsync(int accountId)
		{
			throw new NotImplementedException();
		}

		public Task<Moneytransfer> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<Moneytransfer> GetByReferansNAsync(string referansNo)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<Moneytransfer>> GetTransferHistoryAsync(int accountId, DateTime? started = null, DateTime? finished = null)
		{
			var query = _context.Moneytransfers
			   .Include(t => t.SenderAccount)
			   .Include(t => t.ReceiverAccount)
				.Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId);

			if (started.HasValue)
				query = query.Where(t => t.TransferDate >= started.Value);

			if (finished.HasValue)
				query = query.Where(t => t.TransferDate <= finished.Value);

			return await query.OrderByDescending(t => t.TransferDate).ToListAsync();
		}

		public async Task UpdateAsync(Moneytransfer transfer)
		{
			_context.Moneytransfers.Update(transfer);
		}
	}
}
