using System.Collections.Generic;
namespace Submit_System {
    public interface CheckStyleTester {
        public void SetCheckLocation(string submissin_id);

        public void SetFilesLocation(string path);

        public string RunCheck();

        public CheckStyleResult GetCheckResult();
    }
}