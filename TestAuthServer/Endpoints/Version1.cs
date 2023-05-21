namespace TestAuthServer.Endpoints;

public static class Version1
{
    public static void MapVersion1API(this WebApplication app)
    {
        var v1 = app.MapGroup("api/v1/auth/");
        v1.MapAuthEndpoints();
    }
}
