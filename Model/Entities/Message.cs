using System.Collections.Generic;
using System;
namespace Submit_System {
    public class Message {
        public string ID { get; set; }
        public string SenderID { get; set; }
        public string SenderName { get; set; }
        public DateTime Date { get; set; }
        public string Body { get; set; }
        public string ChatID { get; set; } 
        public bool IsTeacher { get; set; }

    }
}