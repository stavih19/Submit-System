using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Submit_System {
    public class Submission{
        public string ID{get;set;}
        [JsonIgnore]
        public string Exercise_ID{get;set;}
        private string loc;
        [JsonIgnore]
        public string Files_Location {
            get => loc;
            set {
                filenames = FileUtils.GetRelativePaths(value);
                loc = value;
            }
        }
        public List<string> filenames { get; set; }
        public int Auto_Grade{get;set;}
        public int Style_Grade{get;set;}
        public int Manual_Final_Grade{get;set;}
        public string Manual_Check_Data{get;set;}
        public int Submission_Status {get;set;}
        [JsonIgnore]
        public int Submission_Date_Id{get;set;}

        public DateTime Time_Submitted{get;set;}
        [JsonIgnore]
        public string Chat_Late_ID{get;set;}
        [JsonIgnore]
        public string Chat_Appeal_ID{get;set;}
        public bool HasCopied {get; set;} = false;
        public static string GenerateID(string main_submitter_id,string exercise_id){
            return main_submitter_id + "_" + exercise_id;
        }
    }
}