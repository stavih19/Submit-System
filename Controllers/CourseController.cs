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
    public class CourseController : AbstractController
    {    
        private readonly ILogger<ExerciseController> _logger;
        private readonly DatabaseAccess _access;

        public CourseController(ILogger<ExerciseController> logger)
        {
            _logger = logger;
            _access = new DatabaseAccess();
        }

        [Route("Student/CourseList")]
        [HttpGet]
        public ActionResult<List<Course>> StudentCourses(string userid)
        { 
            return _access.GetCourses(userid, CourseRole.Student);
        }
        [Route("Teacher/CourseList")]
        [HttpGet]
        public ActionResult<List<Course>> TeacherCourses(string userid)
        { 
            return _access.GetCourses(userid, CourseRole.Teacher);
        }
        
    }
}

