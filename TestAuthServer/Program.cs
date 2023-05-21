using TestAuthServer.Constants;
using TestAuthServer.Endpoints;
using TestAuthServer.Extensions;
using TestAuthServer.Models.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDependencyServices();

builder.AddAuthenticationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.Use((context, next) =>
//{
//    context.Request.Host = new HostString("localhost:7152");
//    context.Request.Path = new PathString("/api/v1/auth/external-callback"); //if you need this
//    context.Request.Scheme = "https";
//    return next();
//});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapVersion1API();
app.MapGet("/secret", () =>
{
    return "SECRET DATA";
}).RequireAuthorization(policyNames: AuthConstants.Policy.TEST);

app.Run();