namespace TestAuthServer.Constants;

public class AuthConstants
{
    public class Policy
    {
        public const string TEST = "TEST";
        public const string DEFAULT = "DEFAULT";
    }
    
    public class ClaimTypes
    {
        public const string PROVIDER = "AUTH-PROVIDER";
    }

    public class AuthProviders
    {
        public const string GOOGLE = "Google";
        public const string FACEBOOK = "Facebook";
        public const string DEFAULT = "auth.prajwalaradhya.live";
    }
}
