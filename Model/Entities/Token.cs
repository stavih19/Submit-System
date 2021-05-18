using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Submit_System
{
    public class Token
    {
        public string tokenID { get; set; }
        public DateTime EntryDate { get; set; }
        public string UserID { get; set; }
        public bool IsAdmin { get; set; } = false;

        public Token()
        {
            EntryDate = DateTime.Now.AddHours(-2);
        }

        public Token(string userName, string id)
        {
            this.tokenID = id;
            this.EntryDate = DateTime.Now;
            this.UserID = userName;
        }

    }
}
