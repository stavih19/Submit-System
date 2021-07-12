using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Submit_System
{
    public class Token
    {
        public string tokenID { get; set; }
        private long _ticks;
        public DateTime LastEntry {
            get => new DateTime(Interlocked.Read(ref _ticks));
            set => Interlocked.Exchange(ref _ticks, value.Ticks);
        }
        public string UserID { get; set; }
        public bool IsAdmin { get; set; } = false;
        
        public Token(string userName, string id)
        {
            this.tokenID = id;
            this.LastEntry = DateTime.Now;
            this.UserID = userName;
        }
        public Token()
        {
        }

    }
}
