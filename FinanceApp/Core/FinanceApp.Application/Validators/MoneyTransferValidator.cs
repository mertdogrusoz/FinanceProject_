using FinanceApp.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Validators
{
	public class MoneyTransferValidator: AbstractValidator<MoneyTransferDto>
	{
		public MoneyTransferValidator()
		{
			RuleFor(x => x.SenderAccountId)
			   .NotEmpty().WithMessage("Gönderen hesap numarası boş olamaz")
			   .Length(10, 20).WithMessage("Hesap numarası 10-20 karakter arası olmalıdır");

			RuleFor(x => x.ReceiverAccountId)
				.NotEmpty().WithMessage("Alıcı hesap numarası boş olamaz")
				.Length(10, 20).WithMessage("Hesap numarası 10-20 karakter arası olmalıdır")
				.NotEqual(x => x.SenderAccountId).WithMessage("Gönderen ve alıcı hesap aynı olamaz");

			RuleFor(x => x.Amount)
				.GreaterThan(0).WithMessage("Transfer tutarı 0'dan büyük olmalıdır")
				.LessThanOrEqualTo(1000000).WithMessage("Transfer tutarı 1.000.000 TL'yi geçemez");

			RuleFor(x => x.Description)
				.MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir");
		}
	}
}
