using System;
using System.Collections.Generic;
namespace Submit_System {
    public enum SubmissionState { Unsubmitted, Unchecked, Checked, Appeal, AppealChecked }
    public class SubmissionLabel {
        
        public string ID { get; set; }
        public SubmissionState State {get; set; }
        public List<UserLabel> Submitters {get; set; }


    }
}