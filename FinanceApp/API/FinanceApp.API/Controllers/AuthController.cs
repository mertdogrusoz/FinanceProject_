using FinanceApp.Application.DTOs;
using FinanceApp.Application.Services;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FinanceApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
	
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}
		[HttpPost("register")]
		public async Task<ActionResult<AuthResultDto>> Register(RegisterDto registerDto)
		{
			try
			{
				var result = await _authService.RegisterAsync(registerDto);

				if (result.Success)
				{
					return Ok(result);
				}

				return BadRequest(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
		[HttpPost("login")]
		public async Task<ActionResult<AuthResultDto>> Login(LoginDto loginDto)
		{
			try
			{
				var result = await _authService.LoginAsync(loginDto);

				if (result.Success)
				{
					return Ok(result);
				}

				return BadRequest(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
		[HttpPost("logout")]
		[Authorize]
		public async Task<ActionResult> Logout()
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				await _authService.LogoutAsync(userId);
				return Ok(new { message = "Çıkış başarılı" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
		[HttpPost("change-password")]
		[Authorize]
		public async Task<ActionResult<AuthResultDto>> ChangePassword(ChangePasswordDto changePasswordDto)
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

				if (result.Success)
				{
					return Ok(result);
				}

				return BadRequest(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
		[HttpPost("forgot-password")]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
		{
			try
			{
				await _authService.ForgotPasswordAsync(forgotPasswordDto);
				return Ok(new { message = "Şifre sıfırlama bağlantısı email adresinize gönderildi" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
		[HttpPost("reset-password")]
		public async Task<ActionResult<AuthResultDto>> ResetPassword(ResetPasswordDto resetPasswordDto)
		{
			try
			{
				var result = await _authService.ResetPasswordAsync(resetPasswordDto);

				if (result.Success)
				{
					return Ok(result);
				}

				return BadRequest(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
		[HttpGet("profile")]
		[Authorize]
		public async Task<ActionResult<UserDto>> GetProfile()
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var user = await _authService.GetUserByIdAsync(userId);

				if (user == null)
				{
					return NotFound(new { message = "Kullanıcı bulunamadı" });
				}

				return Ok(user);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}



		//[HttpGet]
		//public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllHesaplar()
		//{
		//	try
		//	{
		//		var hesaplar = await _accountService.GetAllAccountsAsync();
		//		return Ok(hesaplar);
		//	}
		//	catch (Exception ex)
		//	{
		//		return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
		//	}
		//}

		//[HttpGet("{id}")]
		//public async Task<ActionResult<AccountDto>> GetHesap(int id)
		//{
		//	try
		//	{
		//		var hesap = await _accountService.GetHesapByIdAsync(id);
		//		return Ok(hesap);
		//	}
		//	catch (AccountNotFoundException ex)
		//	{
		//		return NotFound(new { message = ex.Message });
		//	}
		//	catch (Exception ex)
		//	{
		//		return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
		//	}
		//}

		//[HttpGet("hesap-no/{hesapNo}")]
		//public async Task<ActionResult<AccountDto>> GetHesapByNumara(string hesapNo)
		//{
		//	try
		//	{
		//		var hesap = await _accountService.GetHesapByAccountNumberAsync(hesapNo);
		//		return Ok(hesap);
		//	}
		//	catch (AccountNotFoundException ex)
		//	{
		//		return NotFound(new { message = ex.Message });
		//	}
		//	catch (Exception ex)
		//	{
		//		return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
		//	}
		//}

		//[HttpPost]
		//public async Task<ActionResult<AccountDto>> CreateHesap(CreateAccountDto dto)
		//{
		//	try
		//	{
		//		var hesap = await _accountService.CreateAccountAsync(dto);
		//		return CreatedAtAction(nameof(GetHesap), new { id = hesap.Id }, hesap);
		//	}
		//	catch (ValidationException ex)
		//	{
		//		return BadRequest(new { message = "Validasyon hatası", errors = ex.Message });
		//	}
		//	catch (Exception ex)
		//	{
		//		return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
		//	}
		//}

		//[HttpGet("{hesapNo}/bakiye")]
		//public async Task<ActionResult<decimal>> GetBakiye(string hesapNo)
		//{
		//	try
		//	{
		//		var bakiye = await _accountService.GetBalanceAsync(hesapNo);
		//		return Ok(new { hesapNo, bakiye });
		//	}
		//	catch (AccountNotFoundException ex)
		//	{
		//		return NotFound(new { message = ex.Message });
		//	}
		//	catch (Exception ex)
		//	{
		//		return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
		//	}
		//}

	}
}
