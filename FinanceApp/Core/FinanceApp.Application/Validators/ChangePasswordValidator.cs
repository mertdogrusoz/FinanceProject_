using FinanceApp.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Validators
{
	public class ChangePasswordValidator:AbstractValidator<ChangePasswordDto>
	{
		public ChangePasswordValidator()
		{
			RuleFor(x => x.CurrentPassword)
				.NotEmpty().WithMessage("Mevcut şifre zorunludur");

			RuleFor(x => x.NewPassword)
				.NotEmpty().WithMessage("Yeni şifre zorunludur")
				.MinimumLength(6).WithMessage("Yeni şifre en az 6 karakter olmalıdır")
				.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Yeni şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir");

			RuleFor(x => x.ConfirmNewPassword)
				.NotEmpty().WithMessage("Yeni şifre tekrarı zorunludur")
				.Equal(x => x.NewPassword).WithMessage("Yeni şifreler eşleşmiyor");
		}
	}
}
