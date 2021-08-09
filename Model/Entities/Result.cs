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
        public Result(List<CheckResult> test_results, CheckStyleResult check_style_result){
            this.Test_Results = test_results;
            this.Check_Style_Result= check_style_result;
        }   
        public double GetTestsGrade(){
            if(Test_Results.Count ==0){
                return -1;
            }
            double sum = 0;
            double total = (double) Test_Results.Select(result => result.Weight).Sum();
            foreach(CheckResult c in Test_Results){
                sum = sum + (c.Weight / total) * c.CalculateTestGrade(new BasicOutputComperator());
            }
            return sum;
        }
        public string ToHTML()
        {
            StringBuilder builder = new StringBuilder();
            if(Test_Results?.Any() ?? false)
            {
                double totalTime = Test_Results.Select(result => result.TimeInMs).Sum();
                builder.AppendLine("<u><h3>Automatic Test:</h3></u>");
                builder.AppendLine($"<b>Total grade: {GetTestsGrade()} | Total run time: {totalTime}ms</b>");
                foreach(CheckResult testResult in Test_Results)
                {
                    int grade = testResult.CalculateTestGrade(new BasicOutputComperator());
                    grade *= testResult.Weight / 100;
                    builder.AppendLine("<hr>");
                    builder.AppendLine($"Test: Grade: {grade}/{testResult.Weight} | Run Time: {testResult.TimeInMs}ms");
                    builder.AppendLine("<u><h5>Input:</h5></u>");
                    builder.AppendLine(HttpUtility.HtmlEncode(testResult.Input));
                    builder.AppendLine("<u><h5>Expected output:</h5></u>");
                    builder.AppendLine(HttpUtility.HtmlEncode(testResult.Expected_Output));
                    builder.AppendLine("<u><h5>Your output:</h5></u>");
                    builder.AppendLine(HttpUtility.HtmlEncode(testResult.Output));
                    if(testResult.Is_Error)
                    { 
                        builder.AppendLine("<u><h5>Error:</h5></u>");
                        builder.AppendLine(HttpUtility.HtmlEncode(testResult.Error));
                    }
                    builder.AppendLine("<hr>");
                }
            }
            if(Check_Style_Result != null)
            {
                builder.AppendLine("<u><h3>Coding Style Test:</h3></u>");
                int styleGrade = Check_Style_Result.IsOk ? 100 : 0;
                builder.AppendLine($"Grade: {styleGrade}");
                if(!Check_Style_Result.IsOk)
                {
                    builder.AppendLine("<u><h3>Comments:</h3></u>");
                    builder.Append(Check_Style_Result.Output);
                }
            }
            builder = builder.Replace("\r\n", "<br>").Replace("\n", "<br>");
            return builder.ToString();
        }   
    }
}
