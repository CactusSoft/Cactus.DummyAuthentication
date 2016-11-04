using Microsoft.Owin.Security;

namespace Cactus.DummyAuthentication.Owin
{
    public class DummyAuthenticationOptions : AuthenticationOptions
    {
        public DummyAuthenticationOptions() : base(Const.AuthenticationType)
        {
        }

        public DummyAuthenticationOptions(string authQueryKey) : this()
        {
            AuthQueryKey = authQueryKey;
        }

        /// <summary>
        /// Set if you like to use a query param instead of Authorize header
        /// </summary>
        public string AuthQueryKey { get; set; }
    }
}
