using FinanceApp.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{
	public interface INotificationService
	{
		Task SendAsync(string toEmail, string subject, string htmlBody);
	}
}
