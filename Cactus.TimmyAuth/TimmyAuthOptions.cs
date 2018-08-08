using Microsoft.AspNetCore.Authentication;

namespace Cactus.TimmyAuth
{
    public class TimmyAuthOptions : AuthenticationSchemeOptions
    {
        public TimmyAuthOptions()
        {
        }

        public TimmyAuthOptions(string authQueryKey)
        {
            AuthQueryKey = authQueryKey;
        }

        /// <summary>
        /// Set if you like to use a query param instead of Authorize header
        /// </summary>
        public string AuthQueryKey { get; set; }
    }
}