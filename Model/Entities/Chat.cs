using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Submit_System {
    public enum ChatType { Extension, Appeal }
    public enum ChatState { Open, Rejected, Accepted }
    public class Chat {
        public string ID {get; set; }
        public ChatType Type { get; set; }
        public bool IsClosed { get; set; }
        public ChatState State { get; set; }

    }
}