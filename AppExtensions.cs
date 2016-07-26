using Owin;

namespace Cactus.DummyAuthentication.Owin
{
    public static class AppExtensions
    {
        public static IAppBuilder UseIdentityServerTokenValidation(this IAppBuilder app)
        {
            return app.Use<DummyAuthenticationMiddleware>(app, new DummyAuthenticationOptions());
        }
    }
}
