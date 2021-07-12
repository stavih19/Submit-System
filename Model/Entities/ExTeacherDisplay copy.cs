using System;
using System.Collections.Generic;
namespace Submit_System {
    public class RequestLabel {      
        public string ChatID { get; set; }
        public string StudentID {get; set; }
        public string StudentName {get; set; }
        public string Message {get; set;}
        public ChatType Type { get; set; }
    }
}
