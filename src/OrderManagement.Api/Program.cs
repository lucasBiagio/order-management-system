using Microsoft.OpenApi;
using OrderManagement.Api.Middleware;
using OrderManagement.Application.DependencyInjection;
using OrderManagement.Infrastructure.DependencyInjection;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

var databaseDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Data");

Directory.CreateDirectory(databaseDirectory);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Management API",
        Version = "v1",
        Description =
            "API REST para gerenciamento de clientes, produtos e pedidos."
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

    await DatabaseSeeder.SeedAsync(dbContext);
}

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Order Management API v1");

        options.DocumentTitle = "Order Management API";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program;