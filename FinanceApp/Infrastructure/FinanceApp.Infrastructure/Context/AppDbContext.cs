using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Infrastructure.Context
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Moneytransfer> Moneytransfers { get; set; }
		public DbSet<LoanApplication> LoanApplications { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Hesap Entity Configuration
			modelBuilder.Entity<Account>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
				entity.Property(e => e.IBAN).IsRequired().HasMaxLength(34);
				entity.Property(e => e.BalanceAmount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.AccountOwner).IsRequired().HasMaxLength(100);
				entity.HasIndex(e => e.AccountNumber).IsUnique();
				entity.HasIndex(e => e.IBAN).IsUnique();
			});

			// ParaTransferi Entity Configuration
			modelBuilder.Entity<Moneytransfer>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.ReferenceNumber).IsRequired().HasMaxLength(50);
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.HasIndex(e => e.ReferenceNumber).IsUnique();


			});

			// KrediBasvurusu Entity Configuration
			modelBuilder.Entity<LoanApplication>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.RequestedAmount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.Description).HasMaxLength(1000);

			});

			base.OnModelCreating(modelBuilder);

		}
	}
}
