using Microsoft.AspNetCore.Identity;

namespace TestAuthServer.Models;

public class User : IdentityUser
{
    public User()
    {
        SetAvatarUrl(FullName ?? Guid.NewGuid().ToString("N"));
    }

    public string RefreshToken { get; set; }
    public string AvatarUrl { get; set; }
    public string FullName { get; set; }
    public string AuthProvider { get; set; } = "auth.prajwalaradhya.live";

    public void SetAvatarUrl(string name)
    {
        AvatarUrl = $"https://robohash.org/{FullName}.png?size=100x100";
    }
}
