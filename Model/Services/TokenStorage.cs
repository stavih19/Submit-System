using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Web;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private const string jsonFile = "tokens.json";
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
            if(IsTestMode)
            {
                Deserialize();
            }
        }
        public void Deserialize()
        {
            if(File.Exists(jsonFile))
            {
                Monitor.Enter(jsonFile);
                string json = File.ReadAllText(jsonFile);
                Tokens = JsonSerializer.Deserialize<ConcurrentDictionary<string, Token>>(json);
                Monitor.Exit(jsonFile);
            }
        }
        public void Serialize()
        {
            Monitor.Enter(jsonFile);
            string json = JsonSerializer.Serialize(Tokens);
            using (var reader = File.CreateText(jsonFile))
            {
                reader.Write(json);
            }
            Monitor.Exit(jsonFile);
        }
        public string CreateToken(string userId)
        {
            var token = new Token(userId, GenerateTokenID());
            Tokens[token.tokenID] = token;
            if(IsTestMode)
            {
                Serialize();
            }
            return token.tokenID;
        }
        private string GenerateTokenID()
        {
            string result;
            do {
                result = CryptoUtils.GetRandomBase64String(TOKENLENGTH);
            } while(Tokens.ContainsKey(result));
            return result;
        }
        public void RemoveByID(string userid)
        {
            foreach(Token token in Tokens.Values)
            {
                if(token.UserID == userid)
                {
                    RemoveToken(token.tokenID);
                }
            }
        }
        public bool TryGetUserID(string tokenID, out string username)
        {
            if(IsTestMode)
            {
                username = "576888433";
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
