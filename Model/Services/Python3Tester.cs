using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace Submit_System {
    public class Python3Tester : AutomaticTester
    {
        private List<Test> tests;
        private List<CheckResult> results;

        private string test_location;
        private string files_location;

        public Python3Tester(){
            this.tests = new List<Test>();
            this.results = new List<CheckResult>();
        }

        public void AddTest(Test test)
        {
            this.tests.Add(test);
        }

        public List<CheckResult> GetCheckResults()
        {
            return this.results;
        }

        public int GetGrade()
        {
            double grade = 0;
            foreach(CheckResult result in results){
                double test_grade = result.CalculateTestGrade();
                double test_value = result.Value;
                grade = grade + test_grade*(test_value/100);
            }
            return (int)grade;
        }

        public void RunAllTests()
        {
            foreach(Test test in this.tests){

                //set files **

                string output = "";
                string errors = "";
                double time = 0;

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "/usr/bin/python3";
                start.Arguments = string.Format("{0} {1}", test.Main_Sourse_File, test.GetArgumentsString());
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;

                DateTime started_at = DateTime.Now;
                using(Process process = Process.Start(start))
                {
                    //set stdin
                    StreamWriter stdin = process.StandardInput;
                    if(test.Input.Length > 0){
                        stdin.Write(test.Input);
                    }
                    //wait until timeout:
                    bool exited = process.WaitForExit(1000*test.Timeout_In_Seconds);
                    DateTime ended_at = DateTime.Now;
                    time = (ended_at - started_at).TotalMilliseconds;
                    if(!exited){
                        process.Kill();
                        errors = "Timeout of "+test.Timeout_In_Seconds+" seconds\n";
                    }
                    using(StreamReader reader = process.StandardOutput)
                    {
                        output = reader.ReadToEnd();
                    }
                    using(StreamReader reader = process.StandardError)
                    {
                        errors = errors + reader.ReadToEnd();
                    }
                }
                //special outputs
                if(errors.Length > 0){
                    this.results.Add(new CheckResult(test.Input,output,test.Expected_Output,test.Value,time,errors));
                }else{
                this.results.Add(new CheckResult(test.Input,output,test.Expected_Output,test.Value,time));
                }

                //clear files
            }
        }

        public void SetFilesLocation(string path)
        {
            this.files_location = path;
        }

        public void SetTestLocation(string submissin_id)
        {
            this.test_location = submissin_id;
        }
    }
}