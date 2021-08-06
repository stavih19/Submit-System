using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System;

namespace Submit_System {

    public class Python3TesterFactory : TestManager.TesterFactory
    {
        public AutomaticTester CreateTester()
        {
            return new Python3Tester();
        }

        public CheckStyleTester CreateChecker()
        {
            return new Python3CheckStyleTester();
        }
    }


    public class Python3Tester : AutomaticTester
    {
        private static string exe_file_location = "/usr/bin/python3";
        static Python3Tester()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exe_file_location = MyConfig.Configuration.GetSection("WindowsPythonPath").Value;
            }
        }
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
        
        public string RunAllTests()
        {
            try{
            bool ok;
            string directory_path = Path.Combine(Directory.GetCurrentDirectory(), ("Test_"+test_location));

            foreach(Test test in this.tests){

                if(test.Has_Adittional_Files){
                    ok = CopyAll(test.AdittionalFilesLocation,directory_path);
                    if(!ok){
                    return "Cannot Load files at "+test.AdittionalFilesLocation;
                    }
                }
                ok = CopyAll(files_location,directory_path);
                if(!ok){
                    return "Cannot Load files at "+files_location;
                }

                string output = "";
                string errors = "";
                double time = 0;

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = exe_file_location;
                if(test.ArgumentsString.Length > 0){
                start.Arguments = string.Format("{0} {1}", test.MainSourseFile, test.ArgumentsString);
                } else{
                    start.Arguments = test.MainSourseFile;
                }
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.WorkingDirectory = directory_path;

                DateTime started_at = DateTime.Now;
                using(Process process = Process.Start(start))
                {
                    //set stdin
                    StreamWriter stdin = process.StandardInput;
                    if(test.Input.Length > 0){
                        stdin.Write(test.Input);
                    }
                    //wait until timeout:
                    bool exited = process.WaitForExit(1000*test.TimeoutInSeconds);
                    DateTime ended_at = DateTime.Now;
                    time = (ended_at - started_at).TotalMilliseconds;
                    if(!exited){
                        process.Kill();
                        errors = "Timeout of "+test.TimeoutInSeconds+" seconds\n";
                    }
                    using(StreamReader reader = process.StandardOutput)
                    {
                        output = reader.ReadToEnd();
                    }
                    if(test.OutputFileName != "stdout")
                    {
                        try{
                            output = File.ReadAllText(Path.Combine(directory_path,test.OutputFileName));
                        } catch{
                            output = "";
                            errors = errors + "Cannot read the output file " + test.OutputFileName + ".\n";
                        }
                    }
                    using(StreamReader reader = process.StandardError)
                    {
                        errors = errors + reader.ReadToEnd();
                    }
                }
                if(errors.Length > 0){
                    this.results.Add(new CheckResult(test.Input,output,test.ExpectedOutput,test.Value,time,errors));
                }else{
                    this.results.Add(new CheckResult(test.Input,output,test.ExpectedOutput,test.Value,time));
                }

                Directory.Delete(directory_path,true);
            }
            return "OK";
            }catch(Exception ex){
                return "Exception "+ex.Message;
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

        private static bool CopyAll(string source, string target){
            try
            {
                CopyAll(new DirectoryInfo(source),new DirectoryInfo(target));
                return true;
            } catch{
                return false;
            }
        }
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

    }

    public class Python3CheckStyleTester : CheckStyleTester
    {

        private static string exe_file_location = MyConfig.Configuration.GetSection("PyCodeStylePath").Value;
        private string files_location;
        private string check_location;

        private CheckStyleResult result;
        public void SetFilesLocation(string path)
        {
            this.files_location = path;      }

        public void SetCheckLocation(string submissin_id)
        {
            this.check_location = submissin_id;
        }

        public string RunCheck()
        {
            try{
                bool ok;
                string directory_path = Path.Combine(Directory.GetCurrentDirectory(),("Check_"+check_location));
                ok = CopyAll(files_location,directory_path);
                if(!ok){
                    return "Cannot Load files at "+files_location;
                }
                string output = "";
                string errors = "";

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = exe_file_location;
                start.Arguments = directory_path;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.WorkingDirectory = Directory.GetCurrentDirectory();

                using(Process process = Process.Start(start))
                {
                    //wait until timeout:
                    bool exited = process.WaitForExit(5000);
                    if(!exited){
                        return "Timeout";
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
                output = output.Replace(directory_path, "");
                if(errors.Length > 0){
                    return "Error: "+errors;
                }else{
                    bool is_ok = (output == "");
                    this.result = new CheckStyleResult(is_ok,output);
                }
                Directory.Delete(directory_path,true);
                return "OK";
            }catch(Exception ex){
                return "Exception "+ex.Message;
            }
        }

        public CheckStyleResult GetCheckResult()
        {
            return this.result;
        }

        private static bool CopyAll(string source, string target){
            try
            {
                CopyAll(new DirectoryInfo(source),new DirectoryInfo(target));
                return true;
            } catch{
                return false;
            }
        }
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

    }


}