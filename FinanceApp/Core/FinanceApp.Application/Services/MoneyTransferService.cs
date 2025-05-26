using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using FinanceApp.Domain.Interfaces;
using FinanceApp.Infrastructure.Context;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{

	public class MoneyTransferService : IMoneyTransferService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<SendMoneyDto> _validator;
		private readonly AppDbContext _context;

		public MoneyTransferService(IUnitOfWork unitOfWork, IValidator<SendMoneyDto> validator, AppDbContext context)
		{
			_unitOfWork = unitOfWork;
			_validator = validator;
			_context = context;
		}

		public async Task<Moneytransfer> GetTransferByReferansAsync(string ReferenceNumber)
		{
			if (string.IsNullOrEmpty(ReferenceNumber))
				throw new ArgumentException("Reference number cannot be null or empty", nameof(ReferenceNumber));

			var transfer = await _unitOfWork.MoneyTransferRepository.GetByReferansAsync(ReferenceNumber);

			

			return transfer;
		}

		

		private string GenerateReferansNo()
		{
			return $"TRF{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
		}

		public async Task<TransferResultDto> CreateTransferAsync(SendMoneyDto dto)
		{
			var validationResult = await _validator.ValidateAsync(dto);
			if (!validationResult.IsValid)
				throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var senderAccount = await _unitOfWork.AccountRepository.GetByAccountNumberAsync(dto.SenderAccountId);
				if (senderAccount == null)
					throw new AccountNotFoundException(dto.SenderAccountId);

				var receiverAccount = await _unitOfWork.AccountRepository.GetByAccountNumberAsync(dto.ReceiverAccountId);
				if (receiverAccount == null)
					throw new AccountNotFoundException(dto.ReceiverAccountId);

				if (senderAccount.BalanceAmount < dto.Amount)
					throw new InsufficientBalanceException(dto.SenderAccountId, senderAccount.BalanceAmount, dto.Amount);

				var referansNo = GenerateReferansNo();
				var transfer = new Moneytransfer
				{
					SenderAccountId = senderAccount.Id,
					ReceiverAccountId = receiverAccount.Id,
					Amount = dto.Amount,
					Description = dto.Description,
					Status = TransferStatus.Tamamlandi,
					TransferDate = DateTime.Now,
					ReferenceNumber = referansNo
				};

				senderAccount.BalanceAmount -= dto.Amount;
				receiverAccount.BalanceAmount += dto.Amount;

				await _unitOfWork.MoneyTransferRepository.AddAsync(transfer);
				await _unitOfWork.AccountRepository.UpdateAsync(senderAccount);
				await _unitOfWork.AccountRepository.UpdateAsync(receiverAccount);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new TransferResultDto
				{
					IsSuccessful = true,
					ReferenceNumber = referansNo,
					Message = "Transfer başarıyla tamamlandı",
					TransferDate = DateTime.Now
				};
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

	
		public async Task<IEnumerable<MoneyTransferDto>> GetTransferHistoryAsync(string accountNumber, DateTime? startDate = null, DateTime? endDate = null)
		{
			if (string.IsNullOrEmpty(accountNumber))
				throw new ArgumentException("Account number cannot be null or empty", nameof(accountNumber));

			
			var account = await _unitOfWork.AccountRepository.GetByAccountNumberAsync(accountNumber);
			if (account == null)
				throw new AccountNotFoundException(accountNumber);

		
			if (startDate.HasValue && endDate.HasValue && startDate > endDate)
				throw new ArgumentException("Start date cannot be greater than end date");

			
			if (!startDate.HasValue && !endDate.HasValue)
			{
				startDate = DateTime.Now.AddDays(-30);
				endDate = DateTime.Now;
			}
			else if (!startDate.HasValue)
			{
				startDate = endDate.Value.AddDays(-30);
			}
			else if (!endDate.HasValue)
			{
				endDate = DateTime.Now;
			}

		
			var transfers = await _unitOfWork.MoneyTransferRepository
				.GetTransferHistoryAsync(account.Id, startDate.Value, endDate.Value);

			var transferDtos = transfers.Select(t => new MoneyTransferDto
			{
				Id = t.Id,
				Amount = t.Amount,
				TransferDate = t.TransferDate,
				Description = t.Description,
				SenderAccountNumber = t.SenderAccount?.AccountNumber,
				SenderAccountName = t.SenderAccount?.AccountOwner,
				ReceiverAccountNumber = t.ReceiverAccount?.AccountNumber,
				ReceiverAccountName = t.ReceiverAccount?.AccountOwner,
				TransferType = t.SenderAccountId == account.Id ? "Giden" : "Gelen"
			}).OrderByDescending(t => t.TransferDate);

			return transferDtos;
		}

		
	}
}