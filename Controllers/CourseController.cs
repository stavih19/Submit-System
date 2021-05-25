using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Submit_System;
namespace Submit_System.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class CourseController : AbstractController
    {    
        private readonly ILogger<ExerciseController> _logger;
        private readonly DatabaseAccess _access;

        public CourseController(ILogger<ExerciseController> logger, DatabaseAccess access)
        {
            _logger = logger;
            _access = access;
        }

        [Route("Student/CourseList")]
        [HttpGet]
        public ActionResult<List<Course>> StudentCourses(string userid)
        { 
            return _access.GetCourses(userid, Role.Student);
        }
        [Route("Teacher/CourseList")]
        [HttpGet]
        public ActionResult<List<Course>> TeacherCourses(string userid)
        { 
            return _access.GetCourses(userid, Role.Teacher);
        }
        
    }
}

