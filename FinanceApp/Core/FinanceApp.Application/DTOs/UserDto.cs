﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Application.DTOs
{
	public class UserDto
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsActive { get; set; }
		public string AccountNumber { get; set; }
	}
}
