using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{
	public interface IMoneyTransferService
	{
		Task<TransferResultDto> TransferYapAsync(MoneyTransferDto dto);
		Task<IEnumerable<Moneytransfer>> GetTransferHistoryAsync(string accountNumber, DateTime? started = null, DateTime? finished = null);
		Task<Moneytransfer> GetTransferByReferansAsync(string ReferenceNumber);

	}
}
