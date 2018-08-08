using Microsoft.AspNetCore.Authentication;

namespace Cactus.TimmyAuth
{
    public static class Extensions
    {
        public static AuthenticationBuilder AddTimmyAuth(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<TimmyAuthOptions, TimmyAuthenticationHandler>(Const.AuthenticationType, null);
        }
    }
}
