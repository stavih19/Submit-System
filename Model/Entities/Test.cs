using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Submit_System {
     public class Test{

        public Test(){
            this.Value = 100;
            this.Input = "";
            this.Expected_Output = "";
            this.Output_File_Name = "stdout";
            this.Arguments_String= "";
            this.Timeout_In_Seconds = 300;
            this.Main_Sourse_File = "";
            this.Adittional_Files_Location = "";

        }

        public Test(int id,int value,string input,string expected_output,string output_file_name,string arguments_string,int timeout_in_seconds,string main_sourse_file,string adittional_files_location,string exercise_id,int type){
            this.ID = id;
            this.Value = value;
            this.Input = input;
            this.Expected_Output = expected_output;
            this.Output_File_Name =output_file_name;
            this.Arguments_String = arguments_string;
            this.Timeout_In_Seconds = timeout_in_seconds;
            this.Main_Sourse_File = main_sourse_file;
            this.Adittional_Files_Location = adittional_files_location;
            this.Exercise_ID = exercise_id;
            this.Type = type;
        }
        public int ID {get; set;}
        public int Type{get;set;}
        public string Exercise_ID{get;set;}
        public int Value{get;set;}
        public string Input{get;set;}
        public string Expected_Output{get;set;}
        public string Output_File_Name{get;set;}

        public string Arguments_String{get;set;}

        public int Timeout_In_Seconds{get;set;}

        public string Main_Sourse_File{get;set;}
        public string Adittional_Files_Location { get; set; }
        
        public List<string> AdditionFileNames {get; set; }
        public List<SubmitFile> AdditionalFiles { get; set; } = null; 

        public bool Has_Adittional_Files{get{return Adittional_Files_Location != "";}}
    }

}