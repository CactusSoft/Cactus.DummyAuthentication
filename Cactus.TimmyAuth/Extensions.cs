using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;

namespace Cactus.TimmyAuth
{
    public static class Extensions
    {
        public static AuthenticationBuilder AddTimmyAuth(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<TimmyAuthOptions, TimmyAuthenticationHandler>(Const.AuthenticationType, null);
        }

        /// <summary>
        /// Adds the <see cref="T:Cactus.TimmyAuth.TimmyAuthenticationMiddleware" /> to the specified <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" />, which enables authentication capabilities with multiple authentication schemes support.
        /// </summary>
        /// <param name="app">The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseTimmyAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            return app.UseMiddleware<TimmyAuthenticationMiddleware>(Array.Empty<object>());
        }
    }
}
