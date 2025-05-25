using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using FinanceApp.Domain.Interfaces;
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
		private readonly IValidator<MoneyTransferDto> _validator;

		public MoneyTransferService(IUnitOfWork unitOfWork, IValidator<MoneyTransferDto> validator)
		{
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public Task<Moneytransfer> GetTransferByReferansAsync(string ReferenceNumber)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Moneytransfer>> GetTransferHistoryAsync(string accountNumber, DateTime? started = null, DateTime? finished = null)
		{
			throw new NotImplementedException();
		}
		private string GenerateReferansNo()
		{
			return $"TRF{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
		}

		public async Task<TransferResultDto> TransferYapAsync(MoneyTransferDto dto)
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
					throw new InsufficientBalanceException(dto.SenderAccountId, receiverAccount.BalanceAmount, dto.Amount);

			
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
	}
}
