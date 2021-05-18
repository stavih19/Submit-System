using System;
using System.Collections.Generic;
namespace Submit_System {
    public enum SubmissionState { Unchecked, Checked, Appeal }
    public class SubmissionData {
        
        public string ID { get; set; }
        public int TotalGrade { get; set; }
        public int AutoGrade { get; set; }
        public int StyleGrade { get; set; }
        public int ManualGrade { get; set; }
        public DateTime DateSubmitted {get; set; }
        public Chat ExtensionChat { get; set; } = null;
        public Chat AppealChat { get; set; } = null;
        public List<Student> Submitters {get; set; }
        public SubmitFile Folder {get; set;}

        public SubmissionState State {get; set;}
    }
}