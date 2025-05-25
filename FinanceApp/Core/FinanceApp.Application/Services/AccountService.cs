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
	public class AccountService : IAccountService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateAccountDto> _validator;

		public AccountService(IUnitOfWork unitOfWork, IValidator<CreateAccountDto> validator)
		{
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<AccountDto> CreateAccountAsync(CreateAccountDto dto)
		{
			var validationResult = await _validator.ValidateAsync(dto);
			if (!validationResult.IsValid)
				throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

			var account = new Account
			{
				AccountNumber = GenerateHesapNumarasi(),
				IBAN = GenerateIBAN(),
				AccountOwner = dto.AccountOwner,
				BalanceAmount = dto.FirstAmount,
				CreatedDate = DateTime.Now,
				IsActive = true
			};

			await _unitOfWork.AccountRepository.AddAsync(account);
			await _unitOfWork.SaveChangesAsync();

			return MapToDto(account);
		}

		public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
		{
			var hesaplar = await _unitOfWork.AccountRepository.GetAllAsync();
			return hesaplar.Select(MapToDto);
		}

		public async Task<decimal> GetBalanceAsync(string AccountNumber)
		{
			var account = await _unitOfWork.AccountRepository.GetByAccountNumberAsync(AccountNumber);
			if (account == null)
				throw new AccountNotFoundException(AccountNumber);

			return account.BalanceAmount;
		}

		public async Task<AccountDto> GetHesapByAccountNumberAsync(string AccountNumber)
		{
			var account = await _unitOfWork.AccountRepository.GetByAccountNumberAsync(AccountNumber);
			if (account == null)
				throw new AccountNotFoundException(AccountNumber);

			return MapToDto(account);
		}

		public async Task<AccountDto> GetHesapByIdAsync(int id)
		{
			var account = await _unitOfWork.AccountRepository.GetByIdAsync(id);
			if (account == null)
				throw new AccountNotFoundException(id.ToString());

			return MapToDto(account); ;

		}
		private AccountDto MapToDto(Account account)
		{
			return new AccountDto
			{
				Id = account.Id,
				AccountNumber = account.AccountNumber,
				IBAN = account.IBAN,
				BalanceAmount = account.BalanceAmount,
				AccountOwner = account.AccountOwner,
				CreatedDate = account.CreatedDate,
				IsActive = account.IsActive
			};
		}
		private string GenerateHesapNumarasi()
		{
			return DateTime.Now.Ticks.ToString().Substring(8);
		}

		private string GenerateIBAN()
		{
			var random = new Random();
			return $"TR{random.Next(10, 99)}{random.Next(100000, 999999)}{random.Next(100000000, 999999999)}";
		}
	}
}
