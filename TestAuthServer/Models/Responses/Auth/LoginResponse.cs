using System.Text.Json.Serialization;

namespace TestAuthServer.Models.Responses.Auth;

public record LoginResponse(string message, bool success, string? accessToken = "", string? refreshToken = "", DateTime? expiry = null)
{
    [JsonIgnore]
    public LoginResponseType ResponseType { get; set; } = LoginResponseType.Success;
}

public enum LoginResponseType
{
    Success,
    IncorrectPassword,
    NoUserFound,
    Registered
}