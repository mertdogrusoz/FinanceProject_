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

			
			modelBuilder.Entity<Moneytransfer>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.ReferenceNumber).IsRequired().HasMaxLength(50);
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.HasIndex(e => e.ReferenceNumber).IsUnique();


				entity.HasOne(e => e.SenderAccount)
					.WithMany(h => h.SentTransfers)
					.HasForeignKey(e => e.SenderAccountId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.ReceiverAccount)
					.WithMany(h => h.ReceivedTransfers)
					.HasForeignKey(e => e.ReceiverAccountId)
					.OnDelete(DeleteBehavior.Restrict);


			});

			// KrediBasvurusu Entity Configuration
			modelBuilder.Entity<LoanApplication>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.RequestedAmount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.Description).HasMaxLength(1000);

				entity.HasOne(e => e.Account)
				   .WithMany(h => h.LoansApplications)
				   .HasForeignKey(e => e.AccountId)
				   .OnDelete(DeleteBehavior.Cascade);



			});

			base.OnModelCreating(modelBuilder);

		}
	}
}
