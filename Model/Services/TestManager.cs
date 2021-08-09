using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
namespace Submit_System {

    public class TestManager {

        public interface TesterFactory
        {
            public AutomaticTester CreateTester();
            public CheckStyleTester CreateChecker();
        }

        private static  Dictionary<string,TesterFactory> facts = new Dictionary<string, TesterFactory>();
        
        /*
            Here is the place to add more testers for other languages.
            U can add Factory of the tester by the following syntax:
                facts.Add("name of language",new TesterFactoryOfTheLanguge());
        */
        static TestManager(){
            facts.Add("python3",new Python3TesterFactory());
        }

        public static List<string> GetLanguages(){
            return new List<string>(facts.Keys);
        }
    
        public static (List<CheckResult>,int,string) Test(string submission_id,int test_type){
            (Submission submission,int err,string errstr) = DataBaseManager.ReadSubmission(submission_id);
            if(err != 0){
                return (null,1,"Can't Read Submission "+errstr);
            }
            Exercise exercise;
            (exercise,err,errstr) = DataBaseManager.ReadExercise(submission.ExerciseID);
            if(err != 0){
                return (null,2,"Can't Read Exercise "+errstr);
            }
            List<Test> tests;
            (tests,err,errstr) = DataBaseManager.ReadTestsOfExercise(submission.ExerciseID);
            if(err != 0){
                return (null,3,"Can't Read Tests "+errstr);
            }
            if(!facts.ContainsKey(exercise.ProgrammingLanguage)){
                return (null,4,"Tester doesn't exist");
            }
            AutomaticTester tester = facts[exercise.ProgrammingLanguage].CreateTester();
            tester.SetTestLocation(submission_id);
            foreach(Test t in tests){
                if(t.Type == test_type){
                    tester.AddTest(t);
                }
            }
            tester.SetFilesLocation(submission.FilesLocation);
            errstr = tester.RunAllTests();
            if(errstr != "OK"){
                return (null,5,"Testing process failed "+errstr);
            }
            List<CheckResult> lst = tester.GetCheckResults();
            return (lst,0,"Ok");
        }

        public static (CheckStyleResult,int,string) Check(string submission_id){
            (Submission submission,int err,string errstr) = DataBaseManager.ReadSubmission(submission_id);
            if(err != 0){
                return (null,1,"Can't Read Submission "+errstr);
            }
            Exercise exercise;
            (exercise,err,errstr) = DataBaseManager.ReadExercise(submission.ExerciseID);
            if(err != 0){
                return (null,2,"Can't Read Exercise "+errstr);
            }
            if(!facts.ContainsKey(exercise.ProgrammingLanguage)){
                return (null,6,"Checker doesn't exist");
            }
            CheckStyleTester checker = facts[exercise.ProgrammingLanguage].CreateChecker();
            checker.SetCheckLocation(submission_id);
            checker.SetFilesLocation(submission.FilesLocation);
            errstr = checker.RunCheck();
            if(errstr != "OK"){
                return (null,7,"Checking process failed "+errstr);
            }
            CheckStyleResult result =checker.GetCheckResult();
            return (result,0,"Ok");
        }
        public static Result getTestResults(string submissin_id, int type)
        {
            (var tests, int code, string err) = Test(submissin_id, type);
            switch(code)
            {
                case 0: break;
                case 4: throw new NotImplementedException();
                default: return null;
            }
            CheckStyleResult result;
            (result, code, err) = Check(submissin_id);
            switch(code)
            {
                case 0: break;
                default: return null;
            }
            return new Result(tests, result);
        }
    }
}

