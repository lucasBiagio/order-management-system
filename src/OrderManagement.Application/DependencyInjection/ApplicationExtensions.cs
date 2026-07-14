using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Interfaces.Services;
using OrderManagement.Application.Services;

namespace OrderManagement.Application.DependencyInjection;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddAutoMapper(
            configuration => { },
            typeof(ApplicationAssemblyReference));

        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyReference>();
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}