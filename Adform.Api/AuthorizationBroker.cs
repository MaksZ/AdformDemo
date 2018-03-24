using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adform.Api
{
    /// <summary>
    /// Helper class to get access token
    /// </summary>
    public static class AuthorizationBroker
    {
        public static async Task<UserCredential> AuthorizeAsync(string clientId, string clientSecret)
        {
            using (var client = new TokenClient("https://id.adform.com/sts/connect/token"))
            {
                var form = new Dictionary<string, string>
                    {
                        { "grant_type", "client_credentials" },
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "scope", "https://api.adform.com/scope/eapi" }
                    };

                var token = await client.RequestAsync(form).ConfigureAwait(false);

                return
                    token.ErrorType == ResponseErrorType.None
                    ? UserCredential.FromToken(token.AccessToken, token.TokenType)
                    : UserCredential.Invalid;
            }
        }
    }
}
