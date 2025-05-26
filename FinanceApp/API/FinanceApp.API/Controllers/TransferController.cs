using FinanceApp.Application.DTOs;
using FinanceApp.Application.Services;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinanceApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransferController : ControllerBase
	{
		private readonly IMoneyTransferService _service;

		public TransferController(IMoneyTransferService service)
		{
			_service = service;
		}


		[HttpPost]
		public async Task<ActionResult<TransferResultDto>> TransferYap(SendMoneyDto dto)
		{
			try
			{
				var sonuc = await _service.TransferYapAsync(dto);
				return Ok(sonuc);
			}
			catch (ValidationException ex)
			{
				return BadRequest(new { message = "Validasyon hatası", errors = ex.Message });
			}
			catch (AccountNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (InsufficientBalanceException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}

		[HttpGet("history/{accountNumber}")]
		public async Task<ActionResult<IEnumerable<Moneytransfer>>> GetTransferHistory(
		  string accountNumber,
		  [FromQuery] DateTime? started = null,
		  [FromQuery] DateTime? finished = null)
		{
			try
			{
				var transfers = await _service.GetTransferHistoryAsync(accountNumber, started, finished);
				return Ok(transfers);
			}
			catch (AccountNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (NotImplementedException ex)
			{
				return StatusCode(501, new { message = "Bu özellik henüz implement edilmemiş", error = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}

		[HttpGet("referans/{referenceNumber}")]
		public async Task<ActionResult<Moneytransfer>> GetTransferByReferans(string referenceNumber)
		{
			try
			{
				var transfer = await _service.GetTransferByReferansAsync(referenceNumber);
				if (transfer == null)
					return NotFound(new { message = "Transfer bulunamadı" });
				return Ok(transfer);
			}
			catch (NotImplementedException ex)
			{
				return StatusCode(501, new { message = "Bu özellik henüz implement edilmemiş", error = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
	}
}
