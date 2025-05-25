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
		Task<TransferResultDto> TransferYapAsync(SendMoneyDto dto);
		Task<IEnumerable<MoneyTransferDto>> GetTransferHistoryAsync(string accountNumber, DateTime? startDate = null, DateTime? endDate = null);
		Task<Moneytransfer> GetTransferByReferansAsync(string referenceNumber);

	}
}
