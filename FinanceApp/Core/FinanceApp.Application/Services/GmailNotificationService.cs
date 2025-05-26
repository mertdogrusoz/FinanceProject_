using FinanceApp.Domain.Events;
using FinanceApp.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{
	public class GmailNotificationService : INotificationService
	{
		private readonly GmailSettings _gmail;
		private readonly NotificationSettings _notify;
		private readonly SmtpClient _smtp;

		public GmailNotificationService(
			IOptions<GmailSettings> gmailOpts,
			IOptions<NotificationSettings> notifyOpts)
		{
			_gmail = gmailOpts.Value;
			_notify = notifyOpts.Value;

			_smtp = new SmtpClient("smtp.gmail.com", 587)
			{
				EnableSsl = true,
				Credentials = new NetworkCredential(_gmail.Username, _gmail.Password)
			};
		}

		public async Task NotifyAsync(MoneyTransferedEvent @event)
		{
			var subject = @event.IsCredit
				? $"Hesabınıza {@event.Amount:N2} ₺ Geldi"
				: $"Hesabınızdan {@event.Amount:N2} ₺ Çıktı";

			var body = $@"
				Hesap No       : {@event.AccountNumber}
				E-posta       : {@event.RecipientEmail	}
				Tutar         : {@event.Amount:N2} ₺
				Tarih         : {@event.OccurredAt:dd.MM.yyyy HH:mm}";

			var mail = new MailMessage(
				from: _gmail.Username,
				to: @event.RecipientEmail,
				subject: subject,
				body: body
			);

			await _smtp.SendMailAsync(mail);
		}
	}
}
