using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Submit_System
{
    /// <summary>
    /// Class for storing tokens
    /// </summary>
    public class TokenStorage
    {
        const int TOKENLENGTH = 24; 
        private ConcurrentDictionary<string, Token> Tokens;
        public bool IsTestMode;
        public TokenStorage(bool test=false)
        {
            Tokens = new ConcurrentDictionary<string, Token>();
            IsTestMode = test;
        }
        public string CreateToken(string userId)
        {
            var token = new Token(userId, GenerateTokenID());
            Tokens[token.tokenID] = token;
            return token.tokenID;
        }
        private string GenerateTokenID()
        {
            string result;
            do {
                var tokenBytes = CryptoUtils.GetRandomBytes(TOKENLENGTH);
                result = BitConverter.ToString(tokenBytes).Replace("-", "");
            } while(Tokens.ContainsKey(result));
            return result;
        }
        public bool TryGetUserID(string tokenID, out string username)
        {
            if(IsTestMode)
            {
                username = "Yosi";
                return true;
            };
            if(tokenID == null)
            {
                username = null;
                return false;
            }
            Token token;
            bool result = Tokens.TryGetValue(tokenID, out token);
            if(!result)
            {
                username = null;
                return false;
            }
            username = token.UserID;
            return true;
        }
        public bool IsTokenExist(string tokenID)
        {
            return Tokens.TryGetValue(tokenID, out _);
        }
        public void RemoveToken(string TokenID)
        {
            Tokens.TryRemove(TokenID, out _);
        }
    }
}
