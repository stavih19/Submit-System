using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Submit_System {
     public class Test{

        public Test(){
            this.Value = 100;
            this.Input = "";
            this.ExpectedOutput = "";
            this.OutputFileName = "stdout";
            this.ArgumentsString= "";
            this.TimeoutInSeconds = 300;
            this.MainSourseFile = "";
            this.AdittionalFilesLocation = "";

        }

        public Test(int id,int value,string input,string expected_output,string output_file_name,string arguments_string,int timeout_in_seconds,string main_sourse_file,string adittional_files_location,string exercise_id,int type){
            this.ID = id;
            this.Value = value;
            this.Input = input;
            this.ExpectedOutput = expected_output;
            this.OutputFileName =output_file_name;
            this.ArgumentsString = arguments_string;
            this.TimeoutInSeconds = timeout_in_seconds;
            this.MainSourseFile = main_sourse_file;
            this.AdittionalFilesLocation = adittional_files_location;
            this.ExerciseID = exercise_id;
            this.Type = type;
        }
        public int ID {get; set;}
        public int Type{get;set;}
        public string ExerciseID{get;set;}
        public int Value{get;set;}
        public string Input{get;set;}
        public string ExpectedOutput{get;set;}
        public string OutputFileName{get;set;}

        public string ArgumentsString{get;set;}

        public int TimeoutInSeconds{get;set;}

        public string MainSourseFile{get;set;}
        public string AdittionalFilesLocation { get; set; }
        
        public List<string> AdditionFileNames {get; set; }
        public List<SubmitFile> AdditionalFiles { get; set; } = null; 

        public bool Has_Adittional_Files{get{return AdittionalFilesLocation != "";}}
    }

}