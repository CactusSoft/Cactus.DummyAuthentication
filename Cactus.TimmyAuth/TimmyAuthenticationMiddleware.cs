using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cactus.TimmyAuth
{
    /// <summary>
    /// Default AuthenticationMiddleware from Microsoft.AspNetCore.Authentication, Version=2.2.0.0 with multiple authentication schemes support
    /// </summary>
    public class TimmyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public TimmyAuthenticationMiddleware(RequestDelegate next, IAuthenticationSchemeProvider schemes)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (schemes == null)
                throw new ArgumentNullException(nameof(schemes));
            this._next = next;
            this.Schemes = schemes;
        }

        public IAuthenticationSchemeProvider Schemes { get; set; }

        public async Task Invoke(HttpContext context)
        {
            context.Features.Set<IAuthenticationFeature>((IAuthenticationFeature)new AuthenticationFeature()
            {
                OriginalPath = context.Request.Path,
                OriginalPathBase = context.Request.PathBase
            });
            IAuthenticationHandlerProvider handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (AuthenticationScheme authenticationScheme in await this.Schemes.GetRequestHandlerSchemesAsync())
            {
                IAuthenticationRequestHandler handlerAsync = await handlers.GetHandlerAsync(context, authenticationScheme.Name) as IAuthenticationRequestHandler;
                bool flag = handlerAsync != null;
                if (flag)
                    flag = await handlerAsync.HandleRequestAsync();
                if (flag)
                    return;
            }

            List<AuthenticationScheme> schemes = (await this.Schemes.GetAllSchemesAsync()).ToList();

            // set BearerIdentityServerAuthenticationIntrospection the last because it stops normal IS4 scheme
            schemes.Sort((scheme, authenticationScheme) =>
            {
                if (scheme.Name.Equals("BearerIdentityServerAuthenticationIntrospection", StringComparison.Ordinal) &&
                    scheme.HandlerType.FullName.Equals(
                        "IdentityModel.AspNetCore.OAuth2Introspection.OAuth2IntrospectionHandler",
                        StringComparison.Ordinal))
                    return 1;

                if (authenticationScheme.Name.Equals("BearerIdentityServerAuthenticationIntrospection", StringComparison.Ordinal) &&
                    authenticationScheme.HandlerType.FullName.Equals(
                        "IdentityModel.AspNetCore.OAuth2Introspection.OAuth2IntrospectionHandler",
                        StringComparison.Ordinal))
                    return -1;

                return 0;
            });
            foreach (var authenticateSchemeAsync in schemes)
            {
                if (authenticateSchemeAsync != null)
                {
                    AuthenticateResult authenticateResult = await context.AuthenticateAsync(authenticateSchemeAsync.Name);
                    if (authenticateResult?.Principal != null)
                    {
                        context.User = authenticateResult.Principal;
                        break;
                    }
                }
            }

            await this._next(context);
        }
    }
}
