using Microsoft.Extensions.DependencyInjection;

namespace OrderManagement.Application.DependencyInjection;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddAutoMapper(
            configuration => { },
            typeof(ApplicationAssemblyReference));

        return services;
    }
}