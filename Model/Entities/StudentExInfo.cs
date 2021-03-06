using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Submit_System {
    public class StudentExInfo : GradeCalculator  {
        public int TotalGrade { get => CalculateGrade(); }
        public string SubmissionID { get; set; }
        public Chat ExtensionChat { get; set; }
        public Chat AppealChat { get; set; }
        public int MaxSubmitters { get; set; }
        public List<UserLabel> Submitters {get; set; }
        public string ExID {get; set; }
        public string ExName { get; set; }
        public DateTime DateSubmitted { get; set; }
        public List<SubmitDate> Dates {get; set; }
        public bool IsMultipleSubmission { get; set; }
        public string ManualCheckInfo { get; set; }
        [JsonIgnore]
        public string SubmissionFolder { get; set; }
        public List<string> Filenames {get; set; }
        public SubmissionState State {get; set; }
        public bool IsMainSubmitter { get; set; }

    }
}