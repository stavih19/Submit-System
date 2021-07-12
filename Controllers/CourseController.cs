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
            return HandleDatabaseOutput(_access.GetCourses(userid, Role.Student));
        }
        [Route("Teacher/CourseList")]
        [HttpGet]
        public ActionResult<List<Course>> TeacherCourses(string userid)
        { 
            return HandleDatabaseOutput(_access.GetCourses(userid, Role.Teacher));
        }
        [HttpPost]
        [Route("Teacher/AddTeacher")]
        public ActionResult AddTeacher(string userid, string courseid, string teacherid)
        {
            return HandleDatabaseOutput(_access.AddTeacherToCourse(userid, courseid, teacherid));
        }
        [HttpPost]
        [Route("Teacher/AddChecker")]
        public ActionResult AddChecker(string userid, string courseid, string checkerid)
        {
            return HandleDatabaseOutput(_access.AddCheckerToCourse(userid, courseid, checkerid));
        }
        [HttpDelete]
        [Route("Teacher/DeleteChecker")]
        public ActionResult DeleteChecker(string userid, string courseid, string checkerid)
        {
            return HandleDatabaseOutput(_access.DeleteCheckerFromCourse(userid, courseid, checkerid));
        }
        [HttpGet]
        [Route("Teacher/GetTeachers")]
        public ActionResult<List<UserLabel>> GetTeachers(string userid, string courseid)
        {
            return HandleDatabaseOutput(_access.GetCourseTeachers(userid, courseid));
        }
        [HttpGet]
        [Route("Teacher/GetCheckers")]
        public ActionResult<List<UserLabel>> GetCourseCheckers(string userid, string courseid)
        {
            return HandleDatabaseOutput(_access.GetCourseCheckers(userid, courseid));
        }
    }
}

