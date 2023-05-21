using TestAuthServer.Services;

namespace TestAuthServer.Extensions;

public static class DependencyServices
{
    public static void AddDependencyServices(this IServiceCollection services)
    {
        services.AddAuthServices();
        services.AddServices();
    }

    private static void AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserServices, UserServices>();
    }
}
