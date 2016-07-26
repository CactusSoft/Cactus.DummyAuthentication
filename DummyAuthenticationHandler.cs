using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Cactus.DummyAuthentication.Owin
{
    class DummyAuthenticationHandler : AuthenticationHandler<DummyAuthenticationOptions>
    {
        private readonly ILogger log;

        public DummyAuthenticationHandler(ILogger log)
        {
            this.log = log;
        }

        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var header = Context.Request.Headers["Authorization"];
            if (header != null)
            {
                log.WriteVerbose($"Auth header: {header}");
                if (header.StartsWith(Options.AuthenticationType, StringComparison.OrdinalIgnoreCase))
                {
                    log.WriteVerbose($"{Options.AuthenticationType} auth type detected");
                    var user = header.Substring(Options.AuthenticationType.Length + 1);
                    if (!string.IsNullOrWhiteSpace(user))
                    {
                        var identity = new ClaimsIdentity(Options.AuthenticationType);
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user, null, Options.AuthenticationType));
                        identity.AddClaim(new Claim(ClaimTypes.Name, user));
                        log.WriteInformation($"Authenticated successfully as {user}");
                        return Task.FromResult(new AuthenticationTicket(identity, new AuthenticationProperties()));
                    }
                    log.WriteWarning("No user info found in Authorization header");
                }
                else
                {
                    log.WriteWarning("Unsupported authorization type");
                }
            }
            else
            {
                log.WriteInformation("Authorization header is empty");
            }

            return Task.FromResult<AuthenticationTicket>(null);

        }
    }
}
