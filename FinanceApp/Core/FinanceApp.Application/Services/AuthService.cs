using FinanceApp.Application.DTOs;
using FinanceApp.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
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

		public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IValidator<RegisterDto> registerValidator, IValidator<LoginDto> loginValidator, IValidator<ChangePasswordDto> changePasswordValidator)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
			_registerValidator = registerValidator;
			_loginValidator = loginValidator;
			_changePasswordValidator = changePasswordValidator;
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
			return user != null ? MapToUserDto(user) : null;
		}

		public async Task<UserDto> GetUserByIdAsync(string userId)
		{
			throw new NotImplementedException();
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

			return new AuthResultDto
			{
				Success = true,
				Message = "Giriş başarılı",
				Token = token,
				User = MapToUserDto(user)
			};
		}


		public async Task<bool> LogoutAsync(string userId)
		{
			await _signInManager.SignOutAsync();
			return true;
		}

		public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
		{
			var validationResult = await _registerValidator.ValidateAsync(registerDto);
			if (!validationResult.IsValid)
			{
				return new AuthResultDto
				{
					Success = false,
					Message = "Validasyon hatası",
					Errors = validationResult.Errors.Select(e => e.ErrorMessage)
				};
			}

			var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
			if (existingUser != null)
			{
				return new AuthResultDto
				{
					Success = false,
					Message = "Bu email adresi zaten kullanılıyor",
					Errors = new[] { "Email already exists" }
				};
			}

			var user = new AppUser
			{

				UserName = registerDto.Email,
				Email = registerDto.Email,
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,
				PhoneNumber = registerDto.PhoneNumber,
				CreatedAt = DateTime.UtcNow,
				IsActive = true
			};

			var result = await _userManager.CreateAsync(user, registerDto.Password);

			if (result.Succeeded)
			{
				var token = await GenerateJwtTokenAsync(user);
				return new AuthResultDto
				{
					Success = true,
					Message = "Kayıt başarıyla tamamlandı",
					Token = token,
					User = MapToUserDto(user)
				};
			}

			return new AuthResultDto
			{
				Success = false,
				Message = "Kayıt işlemi başarısız",
				Errors = result.Errors.Select(e => e.Description)
			};
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
		private UserDto MapToUserDto(AppUser user)
		{
			return new UserDto
			{
				Id = user.Id,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				FullName = user.FullName,
				PhoneNumber = user.PhoneNumber,
				CreatedAt = user.CreatedAt,
				IsActive = user.IsActive
			};
		}
	}


}
