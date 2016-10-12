using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Cactus.DummyAuthentication.Owin;
using RestSharp;
using RestSharp.Authenticators;

namespace Cactus.DummyAuthentication.RestSharp
{
    public class DummyAuthenticator : IAuthenticator
    {
        public DummyAuthenticator()
        {
            AuthType = Const.AuthenticationType;
        }

        public DummyAuthenticator(string user) : this()
        {
            User = user;
        }

        public DummyAuthenticator(string user, params KeyValuePair<string, string>[] claims) : this(user)
        {
            Claims = claims;
        }

        public string User { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Claims { get; set; }

        public string AuthType { get; set; }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("Authorization", BuildAuthToken());
        }

        public string BuildAuthToken()
        {
            string token;
            if (Claims != null)
                token = BuildComplexToken();
            else token = User;
            return AuthType + ' ' + token;
        }

        private string BuildComplexToken()
        {
            var res = Const.ComplexTokenMarker + Claims
                .Select(e => Uri.EscapeDataString(e.Key) + '=' + Uri.EscapeDataString(e.Value))
                .Aggregate((a, v) => a + '&' + v);

            if (Claims.Any(e => e.Key != ClaimTypes.NameIdentifier))
            {
                res += '&' + Uri.EscapeDataString(ClaimTypes.NameIdentifier) + '=' + Uri.EscapeDataString(User);
            }

            if (Claims.Any(e => e.Key != ClaimTypes.Name))
            {
                res += '&' + Uri.EscapeDataString(ClaimTypes.Name) + '=' + Uri.EscapeDataString(User);
            }
            return res;
        }
    }
}