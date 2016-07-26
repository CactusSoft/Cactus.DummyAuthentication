using Microsoft.Owin.Security;

namespace Cactus.DummyAuthentication.Owin
{
    public class DummyAuthenticationOptions : AuthenticationOptions
    {
        public DummyAuthenticationOptions() : base(Const.AuthenticationType)
        {
        }
    }
}
