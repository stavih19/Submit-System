using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Submit_System.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class ExerciseController : AbstractController
    {

        private readonly ILogger<ExerciseController> _logger;

        private readonly DatabaseAccess _access;

        public ExerciseController(ILogger<ExerciseController> logger, DatabaseAccess access)
        {
            _logger = logger;
            _access = access;
        }

        [HttpGet]
        [Route("Student/ExerciseLabels")]
        public ActionResult<List<ExerciseLabel>> ExerciseList(string userid, string courseId)
        {
            return HandleDatabaseOutput(_access.GetCourseExercises(courseId, userid, Role.Student));
        }
        [Route("Student/ExerciseList")]
        [HttpGet]
        public ActionResult<List<ExerciseDateDisplay>> ExerciseDates(string userid)
        {
            return HandleDatabaseOutput(_access.GetExcercisesDates(userid));
        }
        [HttpGet]
        [Route("Checker/ExerciseList")]
        public ActionResult<List<CheckerExInfo>> GetCheckerExercises(string userid)
        {
            return _access.GetExercises(userid);
        }
        [HttpGet]
        [Route("Teacher/AllExercises")]
        public ActionResult<SortedDictionary<string, List<ExerciseLabel>>> GetAllExercises(string userid, string courseid)
        {
            return HandleDatabaseOutput(_access.GetAllExercises(userid, courseid));
        }
        [HttpPost]
        [Route("Teacher/CreateExercise")]
        public ActionResult GetCourseCheckers(string userid, string courseid, [FromBody] Exercise exercise)
        {
            return new StatusCodeResult(501);
        }
        [HttpGet]
        [Route("Teacher/ExerciseDetails")]
        public ActionResult<Exercise> GetExerciseDetails(string userid, string exerciseId)
        {
            return null;
        }
        [HttpPost]
        [Route("Teacher/AddExerciseChecker")]
        public ActionResult AddChecker(string userid, string exerciseId, string checkerid)
        {
            return HandleDatabaseOutput(_access.AddCheckerToExercise(userid, exerciseId, checkerid));
        }
        [HttpDelete]
        [Route("Teacher/RemoveExerciseChecker")]
        public ActionResult RemoveChecker(string userid, string exerciseId, string checkerid)
        {
            return HandleDatabaseOutput(_access.DeleteCheckerFromExercise(userid, exerciseId, checkerid));
        }
        [HttpGet]
        [Route("Teacher/ExerciseCheckers")]
        public ActionResult<List<UserLabel>> GetCheckers(string userid, string exerciseId)
        {
            return HandleDatabaseOutput(_access.GetExerciseCheckers(userid, exerciseId));
        }
        [HttpGet]
        [Route("Teacher/ExerciseTests")]
        public ActionResult<List<Test>> GetTests(string userid, string exerciseId)
        {
            return HandleDatabaseOutput(_access.GetTests(userid, exerciseId));
        }
        [HttpPost]
        [Route("Teacher/AddTest")]
        public ActionResult<List<Test>> GetTests(string userid, [FromBody] Test test)
        {
            return HandleDatabaseOutput(_access.AddTest(userid, test));
        }  
        [HttpGet]
        [Route("Teacher/ExerciseList")]
        public ActionResult<List<ExTeacherDisplay>> ExerciseList(string userid)
        {
            return HandleDatabaseOutput(_access.GetTeacherExercises(userid));
        }    
        [HttpGet]
        [Route("Teacher/GetAppeals")]
        public ActionResult<List<RequestLabel>> Appeals(string userid, string exerciseId)
        {
            return HandleDatabaseOutput(_access.GetRequests(exerciseId, userid, ChatType.Appeal));
        }
        [HttpGet]
        [Route("Teacher/GetExtensions")]
        public ActionResult<List<RequestLabel>> Extensions(string userid, string exerciseId)
        {
            return HandleDatabaseOutput(_access.GetRequests(exerciseId, userid, ChatType.Extension));
        }             
    }
}

