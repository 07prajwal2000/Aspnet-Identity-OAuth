using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestAuthServer.Models.Dto;

public class LoginDto
{
    [JsonPropertyName("email"), EmailAddress]
    public string Email { get; set; }

    [JsonPropertyName("password"), MinLength(8)]
    public string Password { get; set; }
}
