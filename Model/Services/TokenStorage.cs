using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace Submit_System
{
    /// <summary>
    /// Class for storing tokens
    /// </summary>
    public class TokenStorage : IDisposable
    {
        const int minute = 60;
        const int TOKENLENGTH = 24; 
        private ConcurrentDictionary<string, Token> Tokens;
        public volatile bool IsTestMode;
        private readonly int ExpirationSeconds;
        private readonly Timer timer;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="test">Test mode or not (in test mode, tokens aren't verified</param>
        /// <param name="exp">Token expiration time in seconds</param>
        public TokenStorage(bool test=false, int exp=30*minute)
        {
            Tokens = new ConcurrentDictionary<string, Token>();
            IsTestMode = test;
            ExpirationSeconds = exp;
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(ExpirationSeconds);
            timer = new System.Threading.Timer((e) =>
            {
                RemoveExpired();
            }, null, startTimeSpan, periodTimeSpan);
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
            var diff = DateTime.Now.Subtract(token.LastEntry).TotalMinutes;
            if(IsExpired(token)) {
               username = null;
               return false;
            }
            token.LastEntry = DateTime.Now;
            username = token.UserID;
            return true;
        }
        /// <summary>
        /// Checks if a token is expired, and removes it it is.
        /// </summary>
        /// <param name="token">The token</param>
        /// <returns>True if the token is expired, false otherwise</returns>
        private bool IsExpired(Token token)
        {
             var diff = DateTime.Now.Subtract(token.LastEntry).TotalSeconds;
             bool expired = diff > ExpirationSeconds;
             if(expired)
             {
                RemoveToken(token.tokenID);
             }
             return expired;
        }
        public bool IsTokenExist(string tokenID)
        {
            if(tokenID == null)
            {
                return false;
            }
            return Tokens.TryGetValue(tokenID, out _);
        }
        public void RemoveToken(string TokenID)
        {
            if(TokenID == null)
            {
                return;
            }
            Tokens.TryRemove(TokenID, out _);
            Trace.WriteLine("Removed token " + TokenID);
        }
        private void RemoveExpired()
        {
            foreach(var token in Tokens.Values)
            {
                IsExpired(token);
            }
        }

        public void Dispose()
        {
            timer.Dispose();
        }
        ~TokenStorage()
        {
           Dispose();
        }
    }
}
