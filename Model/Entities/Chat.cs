using System.Collections.Generic;
namespace Submit_System {
    public enum ChatType { Extension, Appeal }
    public class Chat {
        public string ID {get; set; }
        public ChatType Type { get; set; }
        public bool IsClosed { get; set; }

    }
}