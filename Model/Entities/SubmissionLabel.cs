using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Submit_System {
    public enum SubmissionState { Unsubmitted, Unchecked, Checked, Appeal, AppealChecked }
    public class SubmissionLabel {
        public SubmissionLabel()
        {
            Submitters = new List<UserLabel>();
        }        
        public string ID { get; set; }
        public string CurrentChecker { get; set; }
        public CheckState CheckState { get; set; }
        [JsonIgnore]
        public SubmissionState State {get; set; }
        public List<UserLabel> Submitters {get; set; }


    }
}