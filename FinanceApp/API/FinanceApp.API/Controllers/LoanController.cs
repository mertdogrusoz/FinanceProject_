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
	public class LoanController : ControllerBase
	{
		private readonly ILoanApplicationService _service;

		public LoanController(ILoanApplicationService service)
		{
			_service = service;
		}

		[HttpPost("basvuru")]
		public async Task<ActionResult<int>> CreateBasvuru(LoanApplicationDto dto)
		{
			try
			{
				var basvuruId = await _service.CreateBasvuruAsync(dto);
				return CreatedAtAction(nameof(GetBasvuru), new { id = basvuruId }, new { id = basvuruId });
			}
			catch (ValidationException ex)
			{
				return BadRequest(new { message = "Validasyon hatası", errors = ex.Message });
			}
			catch (AccountNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}

		[HttpGet("basvuru/{id}")]
		public async Task<ActionResult<LoanApplication>> GetBasvuru(int id)
		{
			try
			{
				var basvuru = await _service.GetBasvuruByIdAsync(id);
				if (basvuru == null)
					return NotFound(new { message = "Başvuru bulunamadı" });

				return Ok(basvuru);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}

		[HttpGet("hesap/{hesapNo}")]
		public async Task<ActionResult<IEnumerable<LoanApplication>>> GetBasvurularByHesap(string hesapNo)
		{
			try
			{
				var basvurular = await _service.GetBasvurularByHesapAsync(hesapNo);
				return Ok(basvurular);
			}
			catch (AccountNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}

		[HttpPut("basvuru/{id}/onayla")]
		public async Task<ActionResult> ApproveBasvuru(int id)
		{
			try
			{
				await _service.ApproveBasvuruAsync(id);
				return Ok(new { message = "Başvuru onaylandı" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}

		[HttpPut("basvuru/{id}/reddet")]
		public async Task<ActionResult> RejectBasvuru(int id, [FromBody] string reason)
		{
			try
			{
				await _service.RejectBasvuruAsync(id, reason);
				return Ok(new { message = "Başvuru reddedildi" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
	}
}
