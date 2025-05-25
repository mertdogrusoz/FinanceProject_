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
	public class LoanApplicationService : ILoanApplicationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<LoanApplicationDto> _validator;

		public LoanApplicationService(IUnitOfWork unitOfWork, IValidator<LoanApplicationDto> validator)
		{
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task ApproveBasvuruAsync(int basvuruId)
		{
			var basvuru = await _unitOfWork.LoanApplicationRepository.GetByIdAsync(basvuruId);
			if (basvuru == null)
				throw new Exception("Başvuru bulunamadı");

			basvuru.Status = ApplicationStatus.Approved;
			basvuru.EvaluationDate = DateTime.Now;

			await _unitOfWork.LoanApplicationRepository.UpdateAsync(basvuru);
			await _unitOfWork.SaveChangesAsync();
		}

		public async Task<int> CreateBasvuruAsync(LoanApplicationDto dto)
		{
			var validationResult = await _validator.ValidateAsync(dto);
			if (!validationResult.IsValid)
				throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

			var account = await _unitOfWork.AccountRepository.GetByAccountNumberAsync(dto.AccountNumber);
			if (account == null)
				throw new AccountNotFoundException(dto.AccountNumber);

			var basvuru = new LoanApplication
			{
				AccountId = account.Id,
				RequestedAmount = dto.RequestedAmount,
				TermDuration = dto.TermInMonths,
				LoanType = dto.LoanType,
				Status = ApplicationStatus.Pending,
				ApplicationDate = DateTime.Now,
				Description = dto.Description
			};

			await _unitOfWork.LoanApplicationRepository.AddAsync(basvuru);
			await _unitOfWork.SaveChangesAsync();

			return basvuru.Id;
		}

		public async Task<LoanApplication> GetBasvuruByIdAsync(int id)
		{
			return await _unitOfWork.LoanApplicationRepository.GetByIdAsync(id);
		}

		public async  Task<IEnumerable<LoanApplication>> GetBasvurularByHesapAsync(string hesapNo)
		{
			var hesap = await _unitOfWork.AccountRepository.GetByAccountNumberAsync(hesapNo);
			if (hesap == null)
				throw new AccountNotFoundException(hesapNo);

			return await _unitOfWork.LoanApplicationRepository.GetByAccountIdAsync(hesap.Id);
		}

		public async Task RejectBasvuruAsync(int basvuruId, string reason)
		{
			var basvuru = await _unitOfWork.LoanApplicationRepository.GetByIdAsync(basvuruId);
			if (basvuru == null)
				throw new Exception("Başvuru bulunamadı");

			basvuru.Status = ApplicationStatus.Rejected;
			basvuru.EvaluationDate = DateTime.Now;
			basvuru.Description = reason;

			await _unitOfWork.LoanApplicationRepository.UpdateAsync(basvuru);
			await _unitOfWork.SaveChangesAsync();
		}
	}
}
