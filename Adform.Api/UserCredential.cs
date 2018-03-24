using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adform.Api
{
    public class UserCredential
    {
        public static UserCredential Invalid { get; } = new UserCredential();

        public bool IsValid { get; private set; }
        public string AccessToken { get; private set; }
        public string TokenType { get; private set; }

        internal static UserCredential FromToken(string token, string tokenType)
            =>
                new UserCredential { IsValid = true, AccessToken = token, TokenType = tokenType };

        private UserCredential()
        { }
    }
}
