using FinanceApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.Services
{
	public interface IAuthService
	{
		Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
		Task<AuthResultDto> LoginAsync(LoginDto loginDto);
		Task<AuthResultDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
		Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
		Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
		Task<UserDto> GetUserByIdAsync(string userId);
		Task<UserDto> GetUserByEmailAsync(string email);
		Task<bool> LogoutAsync(string userId);
		Task<Decimal?> GetBalanceAsync(string userId);
	}
}
