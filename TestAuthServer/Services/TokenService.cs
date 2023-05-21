using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestAuthServer.Builders;
using TestAuthServer.Constants;
using TestAuthServer.Models.Settings;

namespace TestAuthServer.Services;

public interface ITokenService
{
    DateTime Expiry { get; set; }

    string GenerateRefreshToken();
    string GenerateToken(params Claim[] claims);
    string GenerateToken(ExternalLoginInfo loginInfo, string provider = AuthConstants.AuthProviders.GOOGLE);
    TokenValidationParameters GetTokenValidationParameters();
}

public class TokenService : ITokenService
{
    private readonly AuthSettings authSettings;

    public DateTime Expiry { get; set; } = DateTime.UtcNow.AddHours(1);

    public TokenService(IOptions<AuthSettings> options) 
        => authSettings = options.Value;

    public string GenerateToken(params Claim[] claims)
    {

        var tokenParams = GetTokenValidationParameters();
        
        var securityToken = new JwtSecurityToken(
            issuer: authSettings.Issuer,
            audience: authSettings.Audience,
            claims: claims,
            expires: Expiry,
            signingCredentials: new SigningCredentials(tokenParams.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
            );

        var token = new JwtSecurityTokenHandler();
        return token.WriteToken(securityToken);
    }

    public string GenerateToken(ExternalLoginInfo loginInfo, string provider = AuthConstants.AuthProviders.GOOGLE)
    {
        var name = loginInfo.Principal.FindFirst(ClaimTypes.Name);
        var email = loginInfo.Principal.FindFirst(ClaimTypes.Email);
        var claims = ClaimsBuilder
            .AddClaim(name!.Type, name!.Value)
            .AddClaim(email!.Type, email!.Value)
            .AddClaim(AuthConstants.ClaimTypes.PROVIDER, provider)
            .Build();

        return GenerateToken(claims);
    }

    public string GenerateRefreshToken() => Guid.NewGuid().ToString("N");

    public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
    {
        var secret = configuration.GetValue<string>("AuthSettings:JwtSecret");
        var issuer = configuration.GetValue<string>("AuthSettings:Issuer");
        var audience = configuration.GetValue<string>("AuthSettings:Audience");

        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ValidAudiences = new[] { audience },
            ValidIssuers = new[] { issuer },

        };
    }
    
    public TokenValidationParameters GetTokenValidationParameters()
    {
        var secret = authSettings.JwtSecret;
        var issuer = authSettings.Issuer;
        var audience = authSettings.Audience;

        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ValidAudience = audience,
            ValidIssuer = issuer,
        };
    }
}
