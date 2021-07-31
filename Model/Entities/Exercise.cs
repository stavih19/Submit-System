using System.Text.Json.Serialization;
using System.Linq;
using System;
using System.Collections.Generic;
namespace Submit_System {
    public class Exercise {

        public void GenerateID(){
            this.ID = this.CourseID + "_" + this.Name;
        }

        public string ID {get; set; }
        public string Name {get; set; }
        [JsonIgnore]
        public string CourseID { get; set; }
        [JsonIgnore]
        public string OriginalExerciseID{ get; set; }

        public int MaxSubmitters { get; set; }
        [JsonIgnore]
        public string FilesLocation { get; set; }
        public int[] Reductions { get; set; }
        [JsonIgnore]
         public string LateSubmissionSettings {
            set => Reductions = ParseLateSettings(value);
            get => MakeLateSettings(Reductions);
        }
        public static int[] ParseLateSettings(string str)
        {
            string[] split = str.Split('_');
            return split.Select(int.Parse).ToArray();
        }
        public static string MakeLateSettings(int[] reductions)
        {
            return String.Join<int>("_", reductions);
        }
        
        
        public string ProgrammingLanguage { get; set; }
        public int AutoTestGradeWeight { get; set; }
        public int StyleTestGradeWeight { get; set; }
        public int IsActive { get; set; }
        public bool MultipleSubmission{ get; set; }
        // number of matches between submissions shown
        public int MossShownMatches { get; set; }
        // number of times a code sequence can be found before it stops being counted
        public int MossMaxTimesMatch { get; set; }
        public string MossLink { get; set; } = "";

        public List<string> Filenames { get; set; }
    }
}