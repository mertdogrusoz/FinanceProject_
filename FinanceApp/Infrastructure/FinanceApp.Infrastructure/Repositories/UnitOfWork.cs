﻿using FinanceApp.Domain.Interfaces;
using FinanceApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{

		private readonly AppDbContext _context;
		private IDbContextTransaction _transaction;

		public UnitOfWork(AppDbContext context)
		{
			_context = context;
			AccountRepository = new AccountRepository(_context);
			MoneyTransferRepository = new MoneyTransferRepository(_context);
			LoanApplicationRepository = new LoanApplicationRepository(_context);
		}

		public IAccountRepository AccountRepository { get; }

		public IMoneyTransferRepository MoneyTransferRepository { get; }

		public ILoanApplicationRepository LoanApplicationRepository { get; }

		public async Task BeginTransactionAsync()
		{
			_transaction = await _context.Database.BeginTransactionAsync();
		}

		public async Task CommitTransactionAsync()
		{
			if (_transaction != null)
			{
				await _transaction.CommitAsync();
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}

		public async void Dispose()
		{
			_transaction?.Dispose();
			_context?.Dispose();
		}

		public async Task RollbackTransactionAsync()
		{
			if (_transaction != null)
			{
				await _transaction.RollbackAsync();
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
