using FinanceApp.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Validators
{
	public class LoanApplicationValidator : AbstractValidator<LoanApplicationDto>
	{
		public LoanApplicationValidator()
		{
			RuleFor(x => x.AccountNumber)
			   .NotEmpty().WithMessage("Hesap numarası boş olamaz");

			RuleFor(x => x.RequestedAmount)
				.GreaterThan(1000).WithMessage("Kredi tutarı en az 1.000 TL olmalıdır")
				.LessThanOrEqualTo(5000000).WithMessage("Kredi tutarı 5.000.000 TL'yi geçemez");

			RuleFor(x => x.TermInMonths)
				.GreaterThan(0).WithMessage("Vade süresi pozitif olmalıdır")
				.LessThanOrEqualTo(360).WithMessage("Vade süresi 360 ayı geçemez");

			RuleFor(x => x.LoanType)
				.IsInEnum().WithMessage("Geçerli bir kredi türü seçiniz");

		}

	}
}
