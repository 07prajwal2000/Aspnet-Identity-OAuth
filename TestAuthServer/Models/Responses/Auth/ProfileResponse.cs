namespace TestAuthServer.Models.Responses.Auth;

public record ProfileResponse(
    string name, 
    string email, 
    bool emailVerified, 
    string avatarUrl, 
    string userId, 
    string username
);
