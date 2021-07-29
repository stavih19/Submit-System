using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Web;
using System.Security;
using System.IO;

namespace Submit_System.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class ExerciseController : AbstractController
    {

        private readonly ILogger<ExerciseController> _logger;
        public ExerciseController(ILogger<ExerciseController> logger, DatabaseAccess access) : base(access)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("Student/ExerciseLabels")]
        public ActionResult<List<ExerciseLabel>> ExerciseList(string courseId)
        {
            DBCode code = _access.CheckCoursePermission(courseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetCourseExercises(courseId));
        }
        [Route("Student/ExerciseList")]
        [HttpGet]
        public ActionResult<List<ExerciseDateDisplay>> ExerciseDates()
        {
            return HandleDatabaseOutput(_access.GetStudentExcercisesDates());
        }
        [HttpGet]
        [Route("Checker/ExerciseList")]
        public ActionResult<List<CheckerExInfo>> GetCheckerExercises()
        {
            return HandleDatabaseOutput(_access.GetCheckerExercises());
        }
        [HttpGet]
        [Route("Teacher/AllExercises")]
        public ActionResult<SortedDictionary<string, List<ExerciseLabel>>> GetAllExercises(string courseid)
        {
             DBCode code = _access.CheckCoursePermission(courseid, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetAllExercises(courseid));
        }
        [HttpPost]
        [Route("Teacher/CreateExercise")]
        public ActionResult<ExOutput> CreateExercise(string courseid, [FromBody] Exercise exercise)
        {
            DBCode code = _access.CheckCoursePermission(courseid, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            exercise.CourseID = courseid;
            string path = System.IO.Path.Combine("Courses", courseid, "Exercises", exercise.Name);
            if(System.IO.Directory.Exists(path))
            {
                return Conflict("Exercise name already in use");
            }
            exercise.FilesLocation = path;
            exercise.GenerateID();
            code = _access.CreateExercise(exercise);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            Directory.CreateDirectory(path);
            var helpFiles = Path.Combine(path, "Help");
            var output = new ExOutput { ID = exercise.ID };
            try
            {
                output.Files = FileUtils.StoreFiles(exercise.HelpFiles, helpFiles, false, false);
            }
            catch(Exception)
            {
                output.Files = new List<string>();
            }
            return output;
        }
        
        [HttpGet]
        [Route("Teacher/ExerciseDetails")]
        public ActionResult<Exercise> GetExerciseDetails(string exerciseId)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetExercise(exerciseId));
        }
        [HttpPost]
        [Route("Teacher/CopyExercise")]
        public ActionResult<string> CopyExercise(string courseid, string oldExerciseId, [FromBody] string exerciseName)
        {
            DBCode code = _access.CheckCoursePermission(courseid, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (Exercise exercise, DBCode code2) = _access.GetExercise(oldExerciseId);
              if(code2 != DBCode.OK)
            {
                return HandleDatabaseOutput(code2);
            }
            exercise.Name = exerciseName;
            string newPath = System.IO.Path.Combine("Courses", courseid, "Exercises", exercise.Name);
            if(System.IO.Directory.Exists(newPath))
            {
                return Conflict("Exercise name already in use");
            }
            FileUtils.CopyDirectory(exercise.FilesLocation, newPath);
            string oldPath = exercise.FilesLocation;
            exercise.FilesLocation = newPath;
            exercise.GenerateID();
            code = _access.CreateExercise(exercise);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (var t, DBCode cod3) = _access.GetTests(oldExerciseId);
            foreach(var test in t)
            {
                test.AdittionalFilesLocation = test.AdittionalFilesLocation.Replace(oldPath, newPath);
                _access.AddTest(test);
            }
            return exercise.ID;
        }
        
       
        [HttpPost]
        [Route("Teacher/AddExerciseChecker")]
        public ActionResult AddChecker(string exerciseId, string checkerid)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.AddCheckerToExercise(exerciseId, checkerid));
        }
        [HttpDelete]
        [Route("Teacher/RemoveExerciseChecker")]
        public ActionResult RemoveChecker(string exerciseId, string checkerid)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.DeleteCheckerFromExercise(exerciseId, checkerid));
        }
        [HttpGet]
        [Route("Teacher/ExerciseCheckers")]
        public ActionResult<List<UserLabel>> GetCheckers(string exerciseId)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetExerciseCheckers(exerciseId));
        }
        [HttpGet]
        [Route("Teacher/ExerciseTests")]
        public ActionResult<List<Test>> GetTests(string exerciseId)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetTests(exerciseId));
        }
        [HttpPost]
        [Route("Teacher/UpdateTest")]
        public ActionResult<TestOutput> UpdateTest([FromBody] Test test)
        {
            DBCode code = _access.CheckTestPerm(test.ID, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.UpdateTest(test));
        }
        [HttpPost]
        [Route("Teacher/DeleteTest")]
        public ActionResult<TestOutput> DeleteTest(int testId)
        {
            DBCode code = _access.CheckTestPerm(testId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.DeleteTest(testId));
        }
        [HttpPost]
        [Route("Teacher/AddTest")]
        public ActionResult<TestOutput> AddTest([FromBody] Test test)
        {
            DBCode code = _access.CheckExercisePermission(test.ExerciseID, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, _) = _access.GetExerciseDirectory(test.ExerciseID);
            path = Path.Combine(path, "Runfiles");
            if(path == null)
            {
                return ServerError();
            }
            string testDir = FileUtils.CreateUniqueDirectory(path, "test");
            test.AdittionalFilesLocation = testDir;
            (int id, DBCode code2) = _access.AddTest(test);
            if(code2 != DBCode.OK)
            {
                Directory.Delete(testDir);
                return ServerError();
            }
            var output = new TestOutput {
                ID = id
            };
            try
            {
                if(test.Has_Adittional_Files)
                {
                    output.Files = FileUtils.StoreFiles(test.AdditionalFiles, testDir, false, true);
                }
            }
            catch
            {
                output.Files = new List<string>();
            }
            return output;
        }  
        [HttpGet]
        [Route("Teacher/ExerciseList")]
        public ActionResult<List<ExTeacherDisplay>> ExerciseList()
        {
            return HandleDatabaseOutput(_access.GetTeacherExercises());
        }    
        [HttpGet]
        [Route("Teacher/AddDate")]
        public ActionResult<int> AddDate(string exerciseId, [FromBody] SubmitDate date)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            date.ExerciseID = exerciseId;
            return HandleDatabaseOutput(_access.AddDate(date));
        }     
        [HttpGet]
        [Route("Teacher/DeleteDate")]
        public ActionResult DeleteDate(int dateId)
        {
            DBCode code = _access.CheckTeacherDatePerm(dateId);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.DeleteDate(dateId));
        }   
        [HttpGet]
        [Route("Teacher/GetDates")]
        public ActionResult<List<TeacherDateDisplay>> GetDates(string exerciseId)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetExerciseDates(exerciseId));
        }
        [NonAction]
        private ActionResult GetHelpFile(string exerciseId, string filename, Role role)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, role);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, DBCode code2) = _access.GetExerciseDirectory(exerciseId);
            path = Path.Combine(path, "Help");
            return FileDownload(path, filename);
        }
        [NonAction]
        private ActionResult DownloadHelpFiles(string exerciseId, string filename, Role role)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, role);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, DBCode code2) = _access.GetExerciseDirectory(exerciseId);
            path = Path.Combine(path, "Help");
            return HandleArchiveSending(path, filename);
        }
        [HttpGet]
        [Route("Teacher/GetHelpFile")]
        public ActionResult TeacherGetHelpFile(string exerciseId, string filename)
        {
            return GetHelpFile(exerciseId, filename, Role.Teacher);
        }
        [HttpGet]
        [Route("Student/GetHelpFile")]
        public ActionResult StudentGetHelpFile(string exerciseId, string filename)
        {
            return GetHelpFile(exerciseId, filename, Role.Student);
        }   
        [HttpGet]
        [Route("Checker/GetHelpFile")]
        public ActionResult CheckerGetHelpFile(string exerciseId, string filename)
        {
            return GetHelpFile(exerciseId, filename, Role.Checker);
        }
         [HttpGet]
        [Route("Teacher/DownloadHelpFiles")]
        public ActionResult TeacherDownloadHelpFiles(string exerciseId, string filename)
        {
            return DownloadHelpFiles(exerciseId, filename, Role.Teacher);
        }
        [HttpGet]
        [Route("Student/DownloadHelpFiles")]
        public ActionResult StudentDownloadHelpFiles(string exerciseId, string filename)
        {
            return DownloadHelpFiles(exerciseId, filename, Role.Student);
        }   
        [HttpGet]
        [Route("Checker/DownloadHelpFiles")]
        public ActionResult CheckerDownloadHelpFiles(string exerciseId, string filename)
        {
            return DownloadHelpFiles(exerciseId, filename, Role.Checker);
        }
        [HttpPost]
        [Route("Teacher/AddHelpFile")]
        public ActionResult<List<string>> AddHelpFile(string exerciseId, [FromBody] List<SubmitFile> files)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, DBCode code2) = _access.GetExerciseDirectory(exerciseId);
            path = Path.Combine(path, "Help");
            try
            {
                return FileUtils.StoreFiles(files, path, false, false);
            }
            catch(SecurityException)
            {
                return BadRequest();
            }
            catch
            {
                return ServerError();
            }
        }
        [HttpDelete]
        [Route("Teacher/DeleteHelpFile")]
        public ActionResult DeleteHelpFile(string exerciseId, string file)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, DBCode code2) = _access.GetExerciseDirectory(exerciseId);
            path = Path.Combine(path, "Help");
            try
            {
                path = FileUtils.PathSafeCombine(path, file);
                System.IO.File.Delete(path);
            }
            catch(SecurityException)
            {
                return BadRequest();
            }
            catch(Exception)
            {
                return ServerError();
            }
            return Ok();
        }
        [HttpDelete]
        [Route("Teacher/DeleteRunFile")]
        public ActionResult DeleteRunFile(int testId, string file)
        {
            DBCode code = _access.CheckTestPerm(testId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, DBCode code2) = _access.GetTestDir(testId);
            if(code2 != DBCode.OK)
            {
                return HandleDatabaseOutput(code2);
            }
            try
            {
                path = FileUtils.PathSafeCombine(path, file);
                System.IO.File.Delete(path);
            }
            catch(SecurityException)
            {
                return BadRequest();
            }
            catch(Exception)
            {
                return ServerError();
            }
            return Ok();
        }
        [HttpPost]
        [Route("Teacher/AddRunFiles")]
        public ActionResult<List<string>> AddRunFiles(int testId, [FromBody] List<SubmitFile> files)
        {
            DBCode code = _access.CheckTestPerm(testId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, DBCode code2) = _access.GetTestDir(testId);
            try
            {
                return FileUtils.StoreFiles(files, path, false, true);
            }
            catch(SecurityException)
            {
                return BadRequest();
            }
            catch
            {
                return ServerError();
            }
        }
        [HttpGet]
        [Route("Teacher/GetRunFile")]
        public ActionResult GetRunFile(int testId, string file)
        {
            DBCode code = _access.CheckTestPerm(testId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, DBCode code2) = _access.GetTestDir(testId);
            if(code2 != DBCode.OK)
            {
                return HandleDatabaseOutput(code2);
            }
            return HandleFileSending(path, file);
        }
        [HttpGet]
        [Route("Teacher/DownloadRunFiles")]
        public ActionResult DownloadRunFiles(int testId, string file)
        {
            DBCode code = _access.CheckTestPerm(testId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            (string path, DBCode code2) = _access.GetTestDir(testId);
            if(code2 != DBCode.OK)
            {
                return HandleDatabaseOutput(code2);
            }
            return HandleArchiveSending(path, file);
        }
    }
}

