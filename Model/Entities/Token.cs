using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Submit_System
{
    public class Token
    {
        public string tokenID { get; set; }
        public DateTime LastEntry { get; set; }
        public string UserID { get; set; }
        public bool IsAdmin { get; set; } = false;

        public Token(string userName, string id)
        {
            this.tokenID = id;
            this.LastEntry = DateTime.Now;
            this.UserID = userName;
        }

    }
}
