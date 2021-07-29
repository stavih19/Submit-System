using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Submit_System
{
    public class Token
    {
        public string tokenID { get; }
        public string UserID { get; }
        public bool IsAdmin { get; }
        private long _ticks;
        public DateTime LastEntry {
            get => new DateTime(Interlocked.Read(ref _ticks));
            set => Interlocked.Exchange(ref _ticks, value.Ticks);
        }
        
        public Token(string userName, string id, bool isAdmin)
        {
            this.tokenID = id;
            this.IsAdmin = isAdmin;
            this.LastEntry = DateTime.Now;
            this.UserID = userName;
        }

    }
}
