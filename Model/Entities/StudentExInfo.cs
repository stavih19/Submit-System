using System;
using System.Collections.Generic;
namespace Submit_System {
    public class StudentExInfo {
        
        public string SubmissionID { get; set; }
        public int TotalGrade { get; set; } = -1;
        public int AutoGrade { get; set; } = -1;
        public int StyleGrade { get; set; } = -1;
        public int ManualGrade { get; set; } = -1;
        public Chat ExtensionChat { get; set; } = null;
        public Chat AppealChat { get; set; } = null;
        public int MaxSubmitters { get; set; }
        public List<Student> Submitters {get; set; }
        public string ExID {get; set; }
        public string ExName { get; set; }
        public DateTime Date {get; set; }
        public int MaxLateDays {get; set; }
        public int LateDayPenalty {get; set; }
        public DateTime DateSubmitted { get; set; }
        public bool IsMultipleSubmission { get; set; }
        public List<string> filenames {get; set; } = null;
        public SubmissionState State {get; set; }
    }
}