namespace Submit_System {
        public class CheckResult{

        public CheckResult(string input,string output,string expected_output,int weight,double time_ms){
            this.Input = input;
            this.Output = output;
            this.Expected_Output = expected_output;
            this.Weight = weight;
            this.Is_Error = false;
            this.Error = null;
            this.TimeInMs = time_ms;
        }

        public CheckResult(string input,string output,string expected_output,int weight,double time_ms,string error){
            this.Input = input;
            this.Output = output;
            this.Expected_Output = expected_output;
            this.Weight = weight;
            this.Is_Error = true;
            this.Error = error;
            this.TimeInMs = time_ms;
        }

        public string Input{get;set;}

        public string Output{get;set;}

        public string Expected_Output{get;set;} 

        public int Weight{get;set;}

        public int CalculateTestGrade(OutputComperator comperator){
            if(this.Is_Error){
                return 0;
            }
            return comperator.CompareOutput(Expected_Output,Output);
        }

        public bool Is_Error{get;set;}

        public string Error{get;set;}

        public double TimeInMs{get;set;}
    }

}