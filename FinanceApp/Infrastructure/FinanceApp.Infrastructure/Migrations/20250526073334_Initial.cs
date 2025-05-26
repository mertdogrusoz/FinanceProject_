using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Tablo var mı kontrol et ve yoksa oluştur
			migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AppUser' AND xtype='U')
                BEGIN
                    CREATE TABLE [AppUser] (
                        [Id] nvarchar(450) NOT NULL,
                        [FirstName] nvarchar(50) NOT NULL,
                        [LastName] nvarchar(50) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
                        [IsActive] bit NOT NULL,
                        [UserName] nvarchar(max) NULL,
                        [NormalizedUserName] nvarchar(max) NULL,
                        [Email] nvarchar(max) NULL,
                        [NormalizedEmail] nvarchar(max) NULL,
                        [EmailConfirmed] bit NOT NULL,
                        [PasswordHash] nvarchar(max) NULL,
                        [SecurityStamp] nvarchar(max) NULL,
                        [ConcurrencyStamp] nvarchar(max) NULL,
                        [PhoneNumber] nvarchar(max) NULL,
                        [PhoneNumberConfirmed] bit NOT NULL,
                        [TwoFactorEnabled] bit NOT NULL,
                        [LockoutEnd] datetimeoffset NULL,
                        [LockoutEnabled] bit NOT NULL,
                        [AccessFailedCount] int NOT NULL,
                        CONSTRAINT [PK_AppUser] PRIMARY KEY ([Id])
                    );
                END
            ");

			// Diğer tabloları da aynı şekilde kontrol ederek oluştur
			migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Accounts' AND xtype='U')
                BEGIN
                    CREATE TABLE [Accounts] (
                        [Id] int IDENTITY(1,1) NOT NULL,
                        [AccountNumber] nvarchar(50) NOT NULL,
                        [IBAN] nvarchar(34) NOT NULL,
                        [BalanceAmount] decimal(18,2) NOT NULL,
                        [AccountOwner] nvarchar(100) NOT NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [IsActive] bit NOT NULL,
                        [UserId] nvarchar(450) NOT NULL,
                        CONSTRAINT [PK_Accounts] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Accounts_AppUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [AppUser] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

			// LoanApplications tablosu
			migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LoanApplications' AND xtype='U')
                BEGIN
                    CREATE TABLE [LoanApplications] (
                        [Id] int IDENTITY(1,1) NOT NULL,
                        [AccountId] int NOT NULL,
                        [RequestedAmount] decimal(18,2) NOT NULL,
                        [TermDuration] int NOT NULL,
                        [LoanType] int NOT NULL,
                        [Status] int NOT NULL,
                        [ApplicationDate] datetime2 NOT NULL,
                        [EvaluationDate] datetime2 NULL,
                        [Description] nvarchar(1000) NOT NULL,
                        CONSTRAINT [PK_LoanApplications] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_LoanApplications_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

			// Moneytransfers tablosu
			migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Moneytransfers' AND xtype='U')
                BEGIN
                    CREATE TABLE [Moneytransfers] (
                        [Id] int IDENTITY(1,1) NOT NULL,
                        [SenderAccountId] int NOT NULL,
                        [ReceiverAccountId] int NOT NULL,
                        [Amount] decimal(18,2) NOT NULL,
                        [Description] nvarchar(500) NOT NULL,
                        [TransferDate] datetime2 NOT NULL,
                        [Status] int NOT NULL,
                        [ReferenceNumber] nvarchar(50) NOT NULL,
                        CONSTRAINT [PK_Moneytransfers] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Moneytransfers_Accounts_ReceiverAccountId] FOREIGN KEY ([ReceiverAccountId]) REFERENCES [Accounts] ([Id]),
                        CONSTRAINT [FK_Moneytransfers_Accounts_SenderAccountId] FOREIGN KEY ([SenderAccountId]) REFERENCES [Accounts] ([Id])
                    );
                END
            ");

			// İndeksleri oluştur
			migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Accounts_AccountNumber')
                    CREATE UNIQUE INDEX [IX_Accounts_AccountNumber] ON [Accounts]([AccountNumber]);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Accounts_IBAN')
                    CREATE UNIQUE INDEX [IX_Accounts_IBAN] ON [Accounts]([IBAN]);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Accounts_UserId')
                    CREATE INDEX [IX_Accounts_UserId] ON [Accounts]([UserId]);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_LoanApplications_AccountId')
                    CREATE INDEX [IX_LoanApplications_AccountId] ON [LoanApplications]([AccountId]);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Moneytransfers_ReceiverAccountId')
                    CREATE INDEX [IX_Moneytransfers_ReceiverAccountId] ON [Moneytransfers]([ReceiverAccountId]);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Moneytransfers_ReferenceNumber')
                    CREATE UNIQUE INDEX [IX_Moneytransfers_ReferenceNumber] ON [Moneytransfers]([ReferenceNumber]);
                
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Moneytransfers_SenderAccountId')
                    CREATE INDEX [IX_Moneytransfers_SenderAccountId] ON [Moneytransfers]([SenderAccountId]);
            ");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "LoanApplications");

			migrationBuilder.DropTable(
				name: "Moneytransfers");

			migrationBuilder.DropTable(
				name: "Accounts");

			migrationBuilder.DropTable(
				name: "AppUser");
		}
	}
}