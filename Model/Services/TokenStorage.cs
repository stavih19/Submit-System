using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Web;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
namespace Submit_System
{
    /// <summary>
    /// Class for storing tokens
    /// </summary>
    public class TokenStorage : IDisposable
    {
        const int TOKENLENGTH = 24; 
        private ConcurrentDictionary<string, Token> _tokens;
        public volatile bool _isTestMode;
        private readonly TimeSpan _expiration;
        private const string TokensFile = "tokens.txt";
        private readonly Timer timer;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="test">Test mode or not (in test mode, tokens aren't verified</param>
        /// <param name="exp">Token expiration time in seconds</param>
        public TokenStorage()
        {
            int time = MyConfig.Configuration.GetSection("ExpirationTime").GetValue<int>("SessionTokenMinutes"); 
            _isTestMode = MyConfig.Configuration.GetValue<bool>("TestMode");
            _tokens = new ConcurrentDictionary<string, Token>();
            var startTimeSpan = TimeSpan.Zero;
            _expiration = TimeSpan.FromMinutes(time);
            timer = new System.Threading.Timer((e) =>
            {
                RemoveExpired();
            }, null, startTimeSpan, _expiration);
            if(_isTestMode)
            {
                Deserialize();
            }
        }
        public void Deserialize()
        {
            if(!File.Exists(TokensFile))
            {
                return;
            }
            lock(TokensFile)
            {
                string content = File.ReadAllText(TokensFile);
                using (var reader = File.OpenText(TokensFile))
                {
                    while(!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        var comps = line.Split(' ');
                        var token = new Token(comps[1], comps[0], Boolean.Parse(comps[2]));
                        _tokens[token.tokenID] = token;
                    }
                }
            }
        }
        public void Serialize()
        {
            Monitor.Enter(TokensFile);
            using (var writer = File.CreateText(TokensFile))
            {
                foreach(var token in _tokens.Values)
                {
                    writer.WriteLine($"{token.tokenID} {token.UserID} {token.IsAdmin}");
                }
            }
            Monitor.Exit(TokensFile);
        }
        public string CreateToken(string userId, bool isAdmin)
        {
            var token = new Token(userId, GenerateTokenID(), isAdmin || _isTestMode);
            _tokens[token.tokenID] = token;
            if(_isTestMode)
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
            } while(_tokens.ContainsKey(result));
            return result;
        }
        public void RemoveByID(string userid)
        {
            foreach(Token token in _tokens.Values)
            {
                if(token.UserID == userid)
                {
                    RemoveToken(token.tokenID);
                }
            }
        }
        public bool TryGetToken(string tokenID, out Token token)
        {
            if(tokenID == null)
            {
                token = null;
                return false;
            }
            bool result = _tokens.TryGetValue(tokenID, out token);
            if(!result)
            {
                token = null;
                return false;
            }
            if(IsExpired(token)) {
               token = null;
               return false;
            }
            token.LastEntry = DateTime.Now;
            return true;
        }
        /// <summary>
        /// Checks if a token is expired, and removes it it is.
        /// </summary>
        /// <param name="token">The token</param>
        /// <returns>True if the token is expired, false otherwise</returns>
        private bool IsExpired(Token token)
        {
             var diff = DateTime.Now.Subtract(token.LastEntry);
             bool expired = diff > _expiration;
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
            return _tokens.TryGetValue(tokenID, out _);
        }
        public void RemoveToken(string TokenID)
        {
            if(TokenID == null)
            {
                return;
            }
            _tokens.TryRemove(TokenID, out _);
            Trace.WriteLine("Removed token " + TokenID);
        }
        private void RemoveExpired()
        {
            foreach(var token in _tokens.Values)
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
