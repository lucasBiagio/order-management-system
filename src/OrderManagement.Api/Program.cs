using OrderManagement.Api.Middleware;
using OrderManagement.Application.DependencyInjection;
using OrderManagement.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var databaseDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Data");

Directory.CreateDirectory(databaseDirectory);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program;