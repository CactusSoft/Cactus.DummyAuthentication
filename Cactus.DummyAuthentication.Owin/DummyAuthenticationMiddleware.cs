using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace Cactus.DummyAuthentication.Owin
{
    public class DummyAuthenticationMiddleware : AuthenticationMiddleware<DummyAuthenticationOptions>
    {
        private readonly ILogger log;

        public DummyAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, DummyAuthenticationOptions options)
            : base(next, options)
        {
            log = app.CreateLogger<DummyAuthenticationMiddleware>();
        }

        // Called for each request, to create a handler for each request.
        protected override AuthenticationHandler<DummyAuthenticationOptions> CreateHandler()
        {
            return new DummyAuthenticationHandler(log);
        }
    }
}
