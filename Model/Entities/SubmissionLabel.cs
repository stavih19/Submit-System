using System;
using System.Collections.Generic;
namespace Submit_System {
    public class SubmissionLabel {
        
        public string ID { get; set; }
        public bool IsChecked { get; set; } = false;
        public bool IsAppeal { get; set; } = false;
        public List<Student> Submitters {get; set; }


    }
}