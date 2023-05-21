using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TestAuthServer.Constants;

namespace TestAuthServer.Extensions;

public static class AuthPolicyExtensions
{
    public static void AddTestPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy(AuthConstants.Policy.TEST, builder =>
        {
            builder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            builder.RequireClaim(ClaimTypes.Email);
        });
    }
    
    public static void AddDefaultPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy(AuthConstants.Policy.DEFAULT, builder =>
        {
            builder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            builder.RequireClaim(ClaimTypes.Email);
        });
    }
}
