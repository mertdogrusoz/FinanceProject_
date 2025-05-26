using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Entities;
using FinanceApp.Infrastructure.Context;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IConfiguration _configuration;
		private readonly IValidator<RegisterDto> _registerValidator;
		private readonly IValidator<LoginDto> _loginValidator;
		private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
		private readonly AppDbContext _context;


		public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IValidator<RegisterDto> registerValidator, IValidator<LoginDto> loginValidator, IValidator<ChangePasswordDto> changePasswordValidator, AppDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
			_registerValidator = registerValidator;
			_loginValidator = loginValidator;
			_changePasswordValidator = changePasswordValidator;
			_context = context;
		}

		public async Task<AuthResultDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
		{
			var validationResult = await _changePasswordValidator.ValidateAsync(changePasswordDto);
			if (!validationResult.IsValid)
			{
				return new AuthResultDto
				{
					Success = false,
					Message = "Validasyon hatası",
					Errors = validationResult.Errors.Select(e => e.ErrorMessage)
				};
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return new AuthResultDto
				{
					Success = false,
					Message = "Kullanıcı bulunamadı",
					Errors = new[] { "User not found" }
				};
			}

			var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

			if (result.Succeeded)
			{
				return new AuthResultDto
				{
					Success = true,
					Message = "Şifre başarıyla değiştirildi"
				};
			}

			return new AuthResultDto
			{
				Success = false,
				Message = "Şifre değiştirme işlemi başarısız",
				Errors = result.Errors.Select(e => e.Description)
			};
		}

		public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
		{
			var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
			if (user == null || !user.IsActive)
			{
				// Security: Don't reveal if user exists
				return true;
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			// TODO: Send email with reset token
			// await _emailService.SendPasswordResetEmailAsync(user.Email, token);

			return true;
		}

		public async Task<UserDto> GetUserByEmailAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return null;

			return await MapToUserDto(user);
		}

		public async Task<UserDto> GetUserByIdAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				if (string.IsNullOrEmpty(userId))
					return null;

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null || !user.IsActive)
				return null;

			// Burada MapToUserDto bir Task<UserDto> döndürüyor,
			// async metodunuzun da gerçek UserDto dönmesi için await edin:
			return await MapToUserDto(user);
		}

		public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
		{
			var validationResult = await _loginValidator.ValidateAsync(loginDto);
			if (!validationResult.IsValid)
			{
				return new AuthResultDto
				{
					Success = false,
					Message = "Validasyon hatası",
					Errors = validationResult.Errors.Select(e => e.ErrorMessage)
				};
			}

			var user = await _userManager.FindByEmailAsync(loginDto.Email);
			if (user == null || !user.IsActive)
			{
				return new AuthResultDto
				{
					Success = false,
					Message = "Geçersiz email veya şifre",
					Errors = new[] { "Invalid credentials" }
				};
			}
				
			// Şifre kontrolü doğrudan UserManager ile yapılır
			var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
			if (!isPasswordValid)
			{
				return new AuthResultDto
				{
					Success = false,
					Message = "Geçersiz email veya şifre",
					Errors = new[] { "Invalid credentials" }
				};
			}

			// Token üretimi
			var token = await GenerateJwtTokenAsync(user);
			var userDto = await MapToUserDto(user);

			return new AuthResultDto
			{
				Success = true,
				Message = "Giriş başarılı",
				Token = token,
				User = userDto
			};
		}


		public async Task<bool> LogoutAsync(string userId)
		{
			await _signInManager.SignOutAsync();
			return true;
		}

		public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
		{
			try
			{
				// 1. Kullanıcıyı oluştur
				var user = new AppUser
				{
					UserName = registerDto.Email,
					Email = registerDto.Email,
					FirstName = registerDto.FirstName,
					LastName = registerDto.LastName,
					// diğer özellikler...
				};

				var result = await _userManager.CreateAsync(user, registerDto.Password);

				if (result.Succeeded)
				{
					// 2. Otomatik hesap oluştur
					var account = new Account
					{
						UserId = user.Id,
						AccountNumber = GenerateAccountNumber(), // Hesap numarası üret
						IBAN = GenerateIBAN(), // IBAN üret
						BalanceAmount = 0, // Başlangıç bakiyesi
						AccountOwner = $"{user.FirstName} {user.LastName}",
						CreatedDate = DateTime.Now,
						IsActive = true
					};

					_context.Accounts.Add(account);
					await _context.SaveChangesAsync();

					// Başarılı sonuç döndür
					return new AuthResultDto
					{
						Success = true,
						Message = "Kullanıcı başarıyla oluşturuldu ve hesap açıldı.",
						UserId = user.Id
					};
				}
				else
				{
					// Kullanıcı oluşturulamadı
					var errors = string.Join(", ", result.Errors.Select(e => e.Description));
					return new AuthResultDto
					{
						Success = false,
						Message = $"Kullanıcı oluşturulamadı: {errors}"
					};
				}
			}
			catch (Exception ex)
			{
				// Hata yönetimi
				return new AuthResultDto
				{
					Success = false,
					Message = $"Bir hata oluştu: {ex.Message}"
				};
			}
		}
		private string GenerateAccountNumber()
		{
			var random = new Random();
			var accountNumber = "";

			for (int i = 0; i < 10; i++)
			{
				accountNumber += random.Next(0, 9).ToString();
			}

			return accountNumber;
		}

		// IBAN üretme fonksiyonu (basit örnek)
		private string GenerateIBAN()
		{
			var random = new Random();
			var iban = "TR";

			// 2 haneli kontrol kodu
			iban += random.Next(10, 99).ToString();

			// 5 haneli banka kodu (örnek: 00001)
			iban += "00001";

			// 1 haneli sıfır
			iban += "0";

			// 16 haneli hesap numarası
			for (int i = 0; i < 16; i++)
			{
				iban += random.Next(0, 9).ToString();
			}

			return iban;
		}
		public async Task<decimal?> GetBalanceAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return null;

			var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive);

			if (account == null)
				return null;

			return account.BalanceAmount;
		}


		public Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
		{
			throw new NotImplementedException();
		}
		private async Task<string> GenerateJwtTokenAsync(AppUser user)
		{
			var jwtSettings = _configuration.GetSection("JwtSettings");
			var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.FullName),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim("FirstName", user.FirstName),
				new Claim("LastName", user.LastName)
			};

		

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["ExpiryInDays"])),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
				Issuer = jwtSettings["Issuer"],
				Audience = jwtSettings["Audience"]
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
		private async Task<UserDto> MapToUserDto(AppUser user)
		{
			var dto = new UserDto
			{
				Id = user.Id,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				FullName = user.FullName,
				CreatedAt = user.CreatedAt,
				IsActive = user.IsActive
				// henüz AccountNumber yok
			};

			
			var account = await _context.Accounts
				.Where(a => a.UserId == user.Id && a.IsActive)
				.FirstOrDefaultAsync();

			dto.AccountNumber = account?.AccountNumber;
			return dto;


		}
	}


}
