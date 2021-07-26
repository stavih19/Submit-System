using System;
namespace Submit_System {
    public class Submission{
        public string ID{get;set;}
        public string Exercise_ID{get;set;}
        public string Files_Location{get;set;}
        public int Auto_Grade{get;set;}
        public int Style_Grade{get;set;}
        public int Manual_Final_Grade{get;set;}
        public string Manual_Check_Data{get;set;}
        public int Submission_Status{get;set;}
        public int Submission_Date_Id{get;set;}

        public DateTime Time_Submitted{get;set;}

        public string Chat_Late_ID{get;set;}
        public string Chat_Appeal_ID{get;set;}

        public string GenerateID(string main_submitter_id,string exercise_id){
            return main_submitter_id + "_" + exercise_id;
        }
    }
}