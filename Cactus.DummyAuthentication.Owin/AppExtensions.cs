using Owin;

namespace Cactus.DummyAuthentication.Owin
{
    public static class AppExtensions
    {
        public static IAppBuilder UseDummyAuthentication(this IAppBuilder app, string authQueryKey = null)
        {
            return app.Use<DummyAuthenticationMiddleware>(app, new DummyAuthenticationOptions(authQueryKey));
        }
    }
}
