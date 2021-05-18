using System;
using System.Collections.Generic;
namespace Submit_System {
    public class SubmissionLabel {
        
        public string ID { get; set; }
        public SubmissionState state {get; set; }
        public List<Student> Submitters {get; set; }


    }
}