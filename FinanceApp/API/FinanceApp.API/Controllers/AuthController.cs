using FinanceApp.Application.DTOs;
using FinanceApp.Application.Services;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
		
		[HttpGet("profile")]
		[Authorize]
		public async Task<ActionResult<UserDto>> GetProfile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userDto = await _authService.GetUserByIdAsync(userId);
			if (userDto == null)
				return NotFound();

			return Ok(userDto);
		}

		[HttpGet("balance")]
		[Authorize]
		public async Task<IActionResult> GetBalance()
		{
			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var balance = await _authService.GetBalanceAsync(userId);

				if (balance == null)
				{
					return NotFound(new { message = "Hesap bulunamadı" });
				}

				return Ok(new { balance });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "İç sunucu hatası", error = ex.Message });
			}
		}
	}
}
