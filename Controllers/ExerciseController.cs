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
    public class ExerciseController : ControllerBase
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
        public ActionResult<List<ExerciseLabel>> ExerciseList(string token, string courseId)
        {
            return _access.GetCourseExercises(courseId);
        }
        [Route("Student/ExerciseList")]
        [HttpGet]
        public ActionResult<List<ExerciseDateDisplay>> ExerciseDates(string token)
        {
            return _access.GetExcercisesDates(token);
        }
    }
}

