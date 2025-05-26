using FinanceApp.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Validators
{
	public class RegisterValidator:AbstractValidator<RegisterDto>
	{
		public RegisterValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email adresi zorunludur")
				.EmailAddress().WithMessage("Geçerli bir email adresi giriniz");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Şifre zorunludur")
				.MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır")
				.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir");

			RuleFor(x => x.ConfirmPassword)
				.NotEmpty().WithMessage("Şifre tekrarı zorunludur")
				.Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor");

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("Ad zorunludur")
				.Length(2, 50).WithMessage("Ad 2-50 karakter arası olmalıdır");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Soyad zorunludur")
				.Length(2, 50).WithMessage("Soyad 2-50 karakter arası olmalıdır");

			RuleFor(x => x.PhoneNumber)
				.Matches(@"^(\+90|0)?[5][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$")
				.When(x => !string.IsNullOrEmpty(x.PhoneNumber))
				.WithMessage("Geçerli bir telefon numarası giriniz");
		}

	}
}
