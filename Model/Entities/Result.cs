using System.Collections.Generic;
using System.Text;
using System.Web;
using System;
using System.Linq;
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
            if(Test_Results.Count ==0){
                return -1;
            }
            double sum = 0;
            foreach(CheckResult c in Test_Results){
                sum = sum + (c.Weight/(double)100) * c.CalculateTestGrade(new BasicOutputComperator());
            }
            return sum;
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            double totalTime = Test_Results.Select(result => result.TimeInMs).Sum();
            builder.AppendLine("Automatic Test:");
            builder.AppendLine($"Total grade: {GetTestsGrade()} | Total run time: {totalTime}ms");
            foreach(CheckResult result in Test_Results)
            {
                int grade = result.CalculateTestGrade(new BasicOutputComperator());
                grade *= result.Weight / 100;
                builder.AppendLine($"________________________________________________");
                builder.AppendLine($"Test: Grade: {grade}/{result.Weight} | Run Time: {result.TimeInMs}ms");
                builder.AppendLine("Input:");
                builder.AppendLine(result.Input);
                builder.AppendLine("Expected output:");
                builder.AppendLine(result.Expected_Output);
                builder.AppendLine($"Your output:\n{result.Output}");
                if(result.Is_Error)
                { 
                    builder.AppendLine($"Error:{result.Error}");
                }
                builder.AppendLine($"________________________________________________");
            }
            return builder.ToString();
        }
    }
}
