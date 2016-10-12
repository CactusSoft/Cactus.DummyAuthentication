using System;
using System.Linq;
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
                    var token = header.Substring(Options.AuthenticationType.Length + 1);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var identity = new ClaimsIdentity(Options.AuthenticationType);
                        if (token[0] == Const.ComplexTokenMarker)
                            FillupComplexIdentity(token.Substring(1), identity);
                        else
                            FillUpSimpleIdentity(token, identity);

                        log.WriteInformation($"Authenticated successfully as {token}");
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

        private void FillupComplexIdentity(string token, ClaimsIdentity identity)
        {
            log.WriteVerbose("Complex token detected");
            foreach (var kvp in token.Split('&').Select(p => p.Split('=')).Where(kvp => kvp.Length >= 2))
            {
                identity.AddClaim(new Claim(Uri.UnescapeDataString(kvp[0]), Uri.UnescapeDataString(kvp[1])));
                log.WriteVerbose($"claim {kvp[0]}:{kvp[1]}");
            }
        }

        private void FillUpSimpleIdentity(string token, ClaimsIdentity identity)
        {
            log.WriteVerbose("Simple token detected, add Name & NameIdentifier claims");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, token, null, Options.AuthenticationType));
            identity.AddClaim(new Claim(ClaimTypes.Name, token));
        }
    }
}
