namespace Submit_System {
    public class CheckStyleResult {

        public CheckStyleResult(bool is_ok,string output){
            this.IsOk = is_ok;
            this.Output = output;
        }
        public bool IsOk{get;set;}
        public string Output{get;set;}
    }
}