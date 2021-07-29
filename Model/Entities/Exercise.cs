using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Submit_System {
    public class Exercise {

        public void GenerateID(){
            this.ID = this.Course_ID + "_" + this.Name;
        }

        public string ID {get; set; }
        public string Name {get; set; }
        [JsonIgnore]
        public string Course_ID { get; set; }
        [JsonIgnore]
        public string Original_Exercise_ID{ get; set; }

        public int MaxSubmitters { get; set; }
        [JsonIgnore]
        public string FilesLocation { get; set; }
        [JsonIgnore]
        public string LateSubmissionSettings { get; set; }
        public string ProgrammingLanguage { get; set; }
        public int AutoTestGradeWeight { get; set; }
        public int StyleTestGradeWeight { get; set; }
        public int IsActive { get; set; }
        public bool MultipleSubmission{ get; set; }
    }
}