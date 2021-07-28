using System.Collections.Generic;
namespace Submit_System {

    public class TestManager {

        public interface TesterFactory
        {
            public AutomaticTester Create();
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
            AutomaticTester tester = facts[exercise.ProgrammingLanguage].Create();
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

    }
}

