using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Entities
{
	public class AppUser:IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName => $"{FirstName} {LastName}";
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public bool IsActive { get; set; } = true;
		

		// Navigation Properties
		public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
	}
}
