using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TestAuthServer.Builders;
using TestAuthServer.Constants;
using TestAuthServer.Models;
using TestAuthServer.Models.Dto;
using TestAuthServer.Models.Responses.Auth;

namespace TestAuthServer.Services;

public interface IUserServices
{
    Task<ProfileResponse> GetProfile(string email);
    Task<LoginResponse> LoginAsync(LoginDto loginDto);
    Task<LoginResponse> LoginWithExternalProvider(ExternalLoginInfo info, string provider = "Google");
    Task<RegisterResponse> RegisterAsync(RegisterDto dto, string provider = "auth.prajwalaradhya.live");
}

public class UserServices : IUserServices
{
    private readonly ITokenService tokenService;
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;

    public UserServices(ITokenService tokenService, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        this.tokenService = tokenService;
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    #region LOGIN LOGIC

    public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
    {
        var (success, msg, responseType) = await ValidateUser(loginDto);
        if (!success)
        {
            return new(msg, false)
            {
                ResponseType = responseType
            };
        }

        var refreshToken = tokenService.GenerateRefreshToken();

        var user = await UpdateRefreshToken(refreshToken, loginDto.Email);

        var claims = ClaimsBuilder
            .AddClaim(ClaimTypes.Email, loginDto.Email)
            .AddClaim(ClaimTypes.Name, user.FullName)
            .Build();

        var jwt = tokenService.GenerateToken(claims: claims);

        return new("Successfully logged in", true, jwt, refreshToken, tokenService.Expiry);
    }

    private async Task<User> UpdateRefreshToken(string token, string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        user!.RefreshToken = token;
        await userManager.UpdateAsync(user);
        return user;
    }

    private async Task<(bool success, string message, LoginResponseType response)> ValidateUser(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
        {
            return (false, "No user found", LoginResponseType.NoUserFound);
        }
        var validPassword = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!validPassword)
        {
            return (false, "Incorrect password", LoginResponseType.IncorrectPassword);
        }
        return (true, string.Empty, LoginResponseType.Success);
    }

    public async Task<LoginResponse> LoginWithExternalProvider(ExternalLoginInfo info, string provider = AuthConstants.AuthProviders.GOOGLE)
    {
        var user = info.Principal;
        var email = user.FindFirst(ClaimTypes.Email)!.Value;
        var name = user.FindFirst(ClaimTypes.Name)!.Value;
        var loginResponseType = LoginResponseType.Success;

        var foundUser = await userManager.FindByEmailAsync(email);
        var refreshToken = tokenService.GenerateRefreshToken();

        if (foundUser is null)
        {
            // TODO: add user to database
            foundUser = new User
            {
                Email = email,
                AuthProvider = provider,
                UserName = email.Split('@')[0],
                FullName = name,
                RefreshToken = refreshToken
            };
            loginResponseType = LoginResponseType.Registered;
            await userManager.CreateAsync(foundUser);
            var result = await userManager.AddLoginAsync(foundUser, info);
            await signInManager.SignInAsync(foundUser, false);
        }

        var claims = ClaimsBuilder
            .AddClaim(ClaimTypes.Email, email)
            .AddClaim(ClaimTypes.Name, name)
            .AddClaim(AuthConstants.ClaimTypes.PROVIDER, provider)
            .Build();

        var token = tokenService.GenerateToken(claims);
        return new LoginResponse("Successfully logged-in", true, token, refreshToken, tokenService.Expiry)
        {
            ResponseType = loginResponseType
        };
    }
    #endregion

    #region REGISTER LOGIC

    public async Task<RegisterResponse> RegisterAsync(RegisterDto dto, string authProvider = "auth.prajwalaradhya.live")
    {
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.UserName,
            AuthProvider = authProvider
        };
        var response = await userManager.CreateAsync(user, dto.Password);

        var errors = response.Errors.Select(x => x.Description).ToArray();

        return new RegisterResponse(errors.Length == 0 ? Array.Empty<string>() : errors, response.Succeeded);
    }

    #endregion

    public async Task<ProfileResponse> GetProfile(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return new ProfileResponse(user!.FullName, user.Email!, user!.EmailConfirmed, user.AvatarUrl, user.Id, user.UserName ?? string.Empty);
    }
}
