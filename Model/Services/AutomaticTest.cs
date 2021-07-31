using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text;
namespace Submit_System {
    public interface AutomaticTester{

        public void SetTestLocation(string submissin_id);
        public void AddTest(Test test);

        public void SetFilesLocation(string path);

        public string RunAllTests();

        public List<CheckResult> GetCheckResults();
    }
}
