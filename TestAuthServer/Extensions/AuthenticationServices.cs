using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestAuthServer.Constants;
using TestAuthServer.Data;
using TestAuthServer.Models;
using TestAuthServer.Services;

namespace TestAuthServer.Extensions;

public static class AuthenticationServices
{
    public static void AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
        var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        builder.Services.AddDbContext<DataContext>(o =>
        {
            o.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });

        builder.Services.AddIdentity<User, IdentityRole>(o =>
        {
            o.Password.RequiredLength = 8;
        })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(c =>
        {
            c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = TokenService.GetTokenValidationParameters(builder.Configuration);
            })
            .AddGoogle(o =>
            {
                o.ClientId = googleClientId;
                o.ClientSecret = googleClientSecret;
                o.Scope.Add("profile");
                o.SignInScheme = IdentityConstants.ExternalScheme;
            });

        builder.Services.AddAuthorization(c =>
        {
            c.AddTestPolicy();
            c.AddDefaultPolicy();
        });
    }

    public static RouteHandlerBuilder WithDefaultPolicyAuthorization(this RouteHandlerBuilder builder)
    {
        builder.RequireAuthorization(policyNames: AuthConstants.Policy.DEFAULT);
        return builder;
    }
}
