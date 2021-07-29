using System;
using System.Collections.Generic;
namespace Submit_System {
    public class Result {
        public List<CheckResult> Test_Results{ get; set; }
        public CheckStyleResult Check_Style_Result{ get; set; }

        public Result(){
            this.Test_Results = null;
            this.Check_Style_Result= null;
        }
        public Result(List<CheckResult> test_results){
            this.Test_Results = test_results;
            this.Check_Style_Result= null;
        }
        public Result(CheckStyleResult check_style_result){
            this.Test_Results = null;
            this.Check_Style_Result= check_style_result;
        }

        public double GetTestsGrade(){
            return this.GetTestsGrade(new BasicOutputComperator());
        }
        public double GetTestsGrade(OutputComperator comperator){
            if(Test_Results.Count ==0){
                return -1;
            }
            double sum = 0;
            foreach(CheckResult c in Test_Results){
                sum = sum + (c.Weight/(double)100) * c.CalculateTestGrade(comperator);
            }
            return sum;
        }

    }
}