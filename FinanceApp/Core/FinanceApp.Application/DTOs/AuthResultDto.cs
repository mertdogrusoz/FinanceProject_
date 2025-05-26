using FinanceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.DTOs
{
	public class AuthResultDto
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public string Token { get; set; }
		public UserDto User { get; set; }
		public string UserId { get; set; }
		public IEnumerable<string> Errors { get; set; } = new List<string>();
	}
}
