using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cactus.TimmyAuth
{
    public class TimmyAuthenticationHandler : AuthenticationHandler<TimmyAuthOptions>
    {
        private readonly IOptionsMonitor<TimmyAuthOptions> _options;
        private readonly ILogger _log;
        public TimmyAuthenticationHandler(IOptionsMonitor<TimmyAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _options = options;
            _log = logger.CreateLogger<TimmyAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var options = _options.CurrentValue;
            var header = Context.Request.Headers["Authorization"].FirstOrDefault();
            try
            {
                AuthenticationTicket ticket;
                if (header != null)
                {
                    _log.LogDebug($"Auth header: {0}", header);
                    ticket = await ProcessAuthValue(header);
                }
                else if(options.AuthQueryKey!=null)
                {
                    var authInfo = Context.Request.Query[options.AuthQueryKey].FirstOrDefault();
                    if (!string.IsNullOrEmpty(options.AuthQueryKey) && authInfo != null)
                    {
                        _log.LogDebug($"Get auth info from query: {authInfo}");
                        ticket = await ProcessAuthValue(authInfo);
                    }
                    else
                    {
                        _log.LogInformation("Authorization header is empty");
                        return AuthenticateResult.NoResult();
                    }
                }
                else
                {
                    _log.LogInformation("No 'Authorization' header, neither query key configuration. No any source of a token found, not authenticated.");
                    return AuthenticateResult.NoResult();
                }

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        protected virtual Task<AuthenticationTicket> ProcessAuthValue(string authValue)
        {
            if (authValue.StartsWith(Scheme.Name, StringComparison.OrdinalIgnoreCase))
            {
                _log.LogDebug("{0} auth type detected", Scheme.Name);
                var token = authValue.Substring(Scheme.Name.Length + 1);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var identity = new ClaimsIdentity(Scheme.Name);
                    if (token[0] == Const.ComplexTokenMarker)
                        FillupComplexIdentity(token.Substring(1), identity);
                    else
                        FillUpSimpleIdentity(token, identity);
                    var principal = new ClaimsPrincipal(identity);
                    _log.LogInformation(($"Authenticated successfully as {token}"));
                    return Task.FromResult(new AuthenticationTicket(principal, Scheme.Name));
                }
                _log.LogWarning("No user info found in Authorization header");
                throw new Exception("No user info found");
            }

            _log.LogDebug("Unsupported authorization type");
            return null;
        }

        private void FillupComplexIdentity(string token, ClaimsIdentity identity)
        {
            _log.LogDebug("Complex token detected");
            foreach (var kvp in token.Split('&').Select(p => p.Split('=')).Where(kvp => kvp.Length >= 2))
            {
                identity.AddClaim(new Claim(Uri.UnescapeDataString(kvp[0]), Uri.UnescapeDataString(kvp[1])));
                _log.LogDebug("claim {0}:{1}", kvp[0], kvp[1]);
            }
        }

        private void FillUpSimpleIdentity(string token, ClaimsIdentity identity)
        {
            _log.LogDebug("Simple token detected, add Name & NameIdentifier claims");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, token, null, Scheme.Name));
            identity.AddClaim(new Claim(ClaimTypes.Name, token));
        }
    }
}