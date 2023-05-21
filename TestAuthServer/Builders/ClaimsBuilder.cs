using System.Security.Claims;

namespace TestAuthServer.Builders;

public interface IClaimsBuilder
{
    IClaimsBuilder AddClaim(string key, string value);
    public IClaimsBuilder AddClaim(Claim claim);
    Claim[] Build();
}

public class ClaimsBuilder
{
    public static IClaimsBuilder AddClaim(string key, string value)
    {
        return new Builder(new Claim(key, value));
    }

    private class Builder : IClaimsBuilder
    {
        private List<Claim> claims = new();

        public Builder(Claim claim)
        {
            claims.Add(claim);
        }

        public IClaimsBuilder AddClaim(string key, string value)
        {
            claims.Add(new Claim(key, value));
            return this;
        }

        public IClaimsBuilder AddClaim(Claim claim)
        {
            claims.Add(claim);
            return this;
        }

        public Claim[] Build() => claims.ToArray();
    }
}
