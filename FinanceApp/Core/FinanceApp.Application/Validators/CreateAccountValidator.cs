using FinanceApp.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Validators
{
	public class CreateAccountValidator : AbstractValidator<CreateAccountDto>
	{
		public CreateAccountValidator()
		{
			RuleFor(x => x.AccountOwner)
			   .NotEmpty().WithMessage("Hesap sahibi adı boş olamaz")
			   .Length(2, 100).WithMessage("Hesap sahibi adı 2-100 karakter arası olmalıdır");

			RuleFor(x => x.FirstAmount)
				.GreaterThanOrEqualTo(0).WithMessage("İlk bakiye negatif olamaz")
				.LessThanOrEqualTo(10000000).WithMessage("İlk bakiye 10.000.000 TL'yi geçemez");
		}

	}
}
