using System.Collections.Generic;
namespace Submit_System {
    public class Message {
        // Needed?
        public string ID {get; set; }
        public string SenderID { get; set; }
        public string SenderName {get; set; }
        public CourseRole SenderRole { get; set;}
        public int Position {get; set; }
        public string Body {get; set; }

    }
}