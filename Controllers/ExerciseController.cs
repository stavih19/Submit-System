using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Submit_System.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    [ServiceFilter(typeof(AuthFilter))]
    public class ExerciseController : AbstractController
    {

        private readonly ILogger<ExerciseController> _logger;

        private readonly DatabaseAccess _access;
        public ExerciseController(ILogger<ExerciseController> logger)
        {
            _logger = logger;
            _access = new DatabaseAccess();
        }

        [HttpGet]
        [Route("Student/ExerciseLabels")]
        public ActionResult<List<ExerciseLabel>> ExerciseList(string userid, string courseId)
        {
            return _access.GetCourseExercises(courseId);
        }
        [Route("Student/ExerciseList")]
        [HttpGet]
        public ActionResult<List<ExerciseDateDisplay>> ExerciseDates(string userid)
        {
            return _access.GetExcercisesDates(userid);
        }
        [HttpGet]
        [Route("Checker/ExerciseList")]
        public ActionResult<List<CheckerExInfo>> GetCheckerExercises(string userid)
        {
            return _access.GetExercises(userid);
        }
        
    }
}

