using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using TestAuthServer.Extensions;
using TestAuthServer.Models;
using TestAuthServer.Models.Dto;
using TestAuthServer.Models.Responses.Auth;
using TestAuthServer.Services;

namespace TestAuthServer.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints(this RouteGroupBuilder routes)
    {
        routes
            .MapPost("login", Login)
            .Produces<LoginResponse>(200)
            .Produces<LoginResponse>(400)
            .Produces<LoginResponse>(401)
            .AllowAnonymous();

        routes
            .MapGet("external-google-login", ExternalLogin)
            .WithDisplayName("Login with Google")
            .WithDescription("Login using google authentication provider");

        routes
            .MapGet("external-callback", ExternalAuthCallback);

        routes
            .MapPost("register", Register)
            .Produces<RegisterResponse>(201)
            .Produces<RegisterResponse>(400)
            .AllowAnonymous();
        
        //routes
        //    .MapGet("sign-out", async (HttpContext ctx, SignInManager<User> signInManager) =>
        //    {
        //        await ctx.SignOutAsync(IdentityConstants.ExternalScheme);
        //        await signInManager.SignOutAsync();
        //        return "ok";
        //    });

        routes
            .MapGet("profile", Profile)
            .Produces<ProfileResponse>(200)
            .Produces(401)
            .WithDefaultPolicyAuthorization();
    }

    private static IResult ExternalLogin(
        [FromServices] SignInManager<User> signInManager,
        [FromQuery] string returnUrl = "/"
        )
    {
        var redirectUrl = $"https://localhost:7152/api/v1/auth/external-callback";
        var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

        //properties.AllowRefresh = true;
        //return Results.Challenge(properties, new List<string> { IdentityConstants.ExternalScheme });
        return Results.Challenge(properties, new List<string> { "Google" });
    }

    private static async Task<IResult> ExternalAuthCallback(
        [FromServices] SignInManager<User> signInManager,
        [FromServices] IUserServices userServices
        )
    {
        var info = await signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            return Results.Redirect("/swagger/index.html?success=false&message=Something went wrong", true);
        }

        var response = await userServices.LoginWithExternalProvider(info);
        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        
        if (!result.Succeeded)
        {
            return Results.Redirect("/swagger/index.html?success=false&message=Failed to login", true);
        }

        return Results.Redirect($"/swagger/index.html?success=true&message={response.message}&token={response.accessToken}&refreshToken={response.refreshToken}&expiry={response.expiry}");
    }

    private static async Task<IResult> Login(
        HttpContext httpContext,
        [FromBody] LoginDto loginDto,
        [FromServices] IUserServices userServices
        )
    {
        var response = await userServices.LoginAsync(loginDto);

        return response.ResponseType switch
        {
            LoginResponseType.Success => Results.Ok(response),

            LoginResponseType.NoUserFound => Results.BadRequest(response),

            LoginResponseType.IncorrectPassword => Results.Content(JsonSerializer.Serialize(response), "application/json", statusCode: StatusCodes.Status401Unauthorized),

            _ => Results.Ok(response),
        };
    }

    private static async Task<IResult> Register(
        [FromBody] RegisterDto dto,
        [FromServices] IUserServices services
        )
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            return Results.BadRequest(new RegisterResponse(new[] { "Password and Confirm Password don't match" }, false));
        }
        var result = await services.RegisterAsync(dto);

        return result.success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<object> Profile(
        ClaimsPrincipal User,
        [FromServices] IUserServices services
        )
    {
        var email = User.FindFirst(ClaimTypes.Email)!;
        var result = await services.GetProfile(email.Value);
        return Results.Ok(result);
    }
}
