using FinanceApp.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Validators
{
	public class LoginValidator:AbstractValidator<LoginDto>
	{
		public LoginValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email adresi zorunludur")
				.EmailAddress().WithMessage("Geçerli bir email adresi giriniz");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Şifre zorunludur");
		}
	}
}
