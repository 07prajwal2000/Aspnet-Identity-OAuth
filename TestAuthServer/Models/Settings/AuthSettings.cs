namespace TestAuthServer.Models.Settings;

public class AuthSettings
{
    public string JwtSecret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
