using FinanceApp.Application.DTOs;
using FinanceApp.Application.Services;
using FinanceApp.Application.Validators;
using FinanceApp.Domain.Interfaces;
using FinanceApp.Infrastructure.Context;
using FinanceApp.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel.Design;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// Application Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMoneyTransferService, MoneyTransferService>();
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();


// FluentValidation
builder.Services.AddScoped<IValidator<CreateAccountDto>, CreateAccountValidator>();
builder.Services.AddScoped<IValidator<MoneyTransferDto>, MoneyTransferValidator>();
builder.Services.AddScoped<IValidator<LoanApplicationDto>, LoanApplicationValidator>();
builder.Services.AddScoped<IValidator<SendMoneyDto>, SendMoneyValidator>();






builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReact",
		policy =>
		{
			policy.WithOrigins("http://127.0.0.1:5500") 
				  .AllowAnyHeader()
				  .AllowAnyMethod();
		});
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors("AllowReact");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
