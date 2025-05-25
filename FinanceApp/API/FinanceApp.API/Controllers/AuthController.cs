using FinanceApp.Application.DTOs;
using FinanceApp.Application.Services;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace FinanceApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAccountService _accountService;

		public AuthController(IAccountService accountService)
		{
			_accountService = accountService;
		}
		[HttpGet]
		public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllHesaplar()
		{
			try
			{
				var hesaplar = await _accountService.GetAllAccountsAsync();
				return Ok(hesaplar);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AccountDto>> GetHesap(int id)
		{
			try
			{
				var hesap = await _accountService.GetHesapByIdAsync(id);
				return Ok(hesap);
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

		[HttpGet("hesap-no/{hesapNo}")]
		public async Task<ActionResult<AccountDto>> GetHesapByNumara(string hesapNo)
		{
			try
			{
				var hesap = await _accountService.GetHesapByAccountNumberAsync(hesapNo);
				return Ok(hesap);
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

		[HttpPost]
		public async Task<ActionResult<AccountDto>> CreateHesap(CreateAccountDto dto)
		{
			try
			{
				var hesap = await _accountService.CreateAccountAsync(dto);
				return CreatedAtAction(nameof(GetHesap), new { id = hesap.Id }, hesap);
			}
			catch (ValidationException ex)
			{
				return BadRequest(new { message = "Validasyon hatası", errors = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}

		[HttpGet("{hesapNo}/bakiye")]
		public async Task<ActionResult<decimal>> GetBakiye(string hesapNo)
		{
			try
			{
				var bakiye = await _accountService.GetBalanceAsync(hesapNo);
				return Ok(new { hesapNo, bakiye });
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

	}
}
