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
    public class CourseController : ControllerBase
    {

        private readonly ILogger<ExerciseController> _logger;

        private readonly DatabaseAccess _access;

        public CourseController(ILogger<ExerciseController> logger)
        {
            _logger = logger;
            _access = new DatabaseAccess();
        }

        [Route("Student/")]
        [HttpGet]
        public ActionResult<List<Course>> CourseList(string token)
        { 
            return _access.GetCourses(token, CourseRole.Student);
        }
    }
}

