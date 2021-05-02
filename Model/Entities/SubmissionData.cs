using System;
using System.Collections.Generic;
namespace Submit_System {
    public class FullSubmission {
        
        public string ID { get; set; }
        public int TotalGrade { get; set; }
        public int AutoGrade { get; set; }
        public int StyleGrade { get; set; }
        public int ManualGrade { get; set; }
        public string ExtensionChatID { get; set; }
        public string AppealChatID { get; set; }
        public List<Student> Submitters {get; set; }
    }
}