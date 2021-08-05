using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Submit_System {
    public class Submission{
        public string ID{get;set;}
        [JsonIgnore]
        public string ExerciseID{get;set;}
        [JsonIgnore]
        public string FilesLocation { get; set; }
        public List<string> Filenames { get; set; }
        public int AutoGrade{get;set;}
        public int StyleGrade{get;set;}
        public int ManualFinalGrade{get;set;}
        public string ManualCheckData{get;set;}
        public int SubmissionStatus {get;set;}
        [JsonIgnore]
        public int SubmissionDateId{get;set;}

        public DateTime TimeSubmitted{get;set;}
        public bool HasCopied {get; set;} = false;
        [JsonIgnore]
        public string CurrentCheckerId {get; set;}
        public CheckState CheckState { get; set; }
        public int TotalGrade { get; set; }
        public static string GenerateID(string main_submitter_id,string exercise_id){
            return main_submitter_id + "_" + exercise_id;
        }
    }
}