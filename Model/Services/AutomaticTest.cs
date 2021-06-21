using System.Collections.Generic;
namespace Submit_System {


    public class Test{

        public Test(){
            this.Value = 100;
            this.Input = "";
            this.Expected_Output = "";
            this.Output_File_Name = "stdout";
            this.ArgumentsString= "";
            this.Timeout_In_Seconds = 300;
            this.Main_Sourse_File = null;
            this.AdittionalFilesLocation = null;

        }

        public Test(int value,string input,string expected_output,string output_file_name,string arguments_string,int timeout_in_seconds,string main_sourse_file,string adittional_files_location,string exercise_id,int type){
            this.Value = value;
            this.Input = input;
            this.Expected_Output = expected_output;
            this.Output_File_Name =output_file_name;
            this.ArgumentsString = arguments_string;
            this.Timeout_In_Seconds = timeout_in_seconds;
            this.Main_Sourse_File = main_sourse_file;
            this.AdittionalFilesLocation = adittional_files_location;
            this.Exercise_ID = exercise_id;
            this.Type = type;
        }

        public int Type{get;set;}
        public string Exercise_ID{get;set;}
        public int Value{get;set;}
        public string Input{get;set;}
        public string Expected_Output{get;set;}
        public string Output_File_Name{get;set;}

        public string ArgumentsString{get;set;}

        public int Timeout_In_Seconds{get;set;}

        public string Main_Sourse_File{get;set;}

        public string AdittionalFilesLocation{get;set;}

        public bool Has_Adittional_Files{get{return AdittionalFilesLocation != null;}}
    }

    public class CheckResult{

        public CheckResult(string input,string output,string expected_output,int value,double time_ms){
            this.Input = input;
            this.Output = output;
            this.Expected_Output = expected_output;
            this.Value = value;
            this.Is_Error = false;
            this.Error = null;
            this.TimeInMs = time_ms;
        }

        public CheckResult(string input,string output,string expected_output,int value,double time_ms,string error){
            this.Input = input;
            this.Output = output;
            this.Expected_Output = expected_output;
            this.Value = value;
            this.Is_Error = true;
            this.Error = error;
            this.TimeInMs = time_ms;
        }

        public string Input{get;set;}

        public string Output{get;set;}

        public string Expected_Output{get;set;} 

        public int Value{get;set;}

        public int CalculateTestGrade(){
            if(this.Is_Error){
                return 0;
            }
            if(Output == Expected_Output){
                return 100;
            }
            return 0;
        }

        public bool Is_Error{get;set;}

        public string Error{get;set;}

        public double TimeInMs{get;set;}
    }



    public interface AutomaticTester{

        public void SetTestLocation(string submissin_id);
        public void AddTest(Test test);

        public bool RunAllTests();

        public void SetFilesLocation(string path);

        public int GetGrade();

        public List<CheckResult> GetCheckResults();
    }
}