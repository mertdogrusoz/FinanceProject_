using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{
	public interface ILoanApplicationService
	{
		Task<int> CreateBasvuruAsync(LoanApplicationDto dto);
		Task<LoanApplication> GetBasvuruByIdAsync(int id);
		Task<IEnumerable<LoanApplication>> GetBasvurularByHesapAsync(string hesapNo);
		Task ApproveBasvuruAsync(int basvuruId);
		Task RejectBasvuruAsync(int basvuruId, string reason);
	}
}
