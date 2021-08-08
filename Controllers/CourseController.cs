using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
namespace Submit_System.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class CourseController : AbstractController
    {    

        public CourseController(DatabaseAccess access) : base(access) {}

        [Route("Student/CourseList")]
        [HttpGet]
        public ActionResult<List<Course>> StudentCourses()
        { 
            return HandleDatabaseOutput(_access.GetCourses(Role.Student));
        }
        [Route("Teacher/CourseList")]
        [HttpGet]
        public ActionResult<List<Course>> TeacherCourses()
        { 
            return HandleDatabaseOutput(_access.GetCourses(Role.Teacher));
        }
        [HttpPost]
        [Route("Teacher/AddTeacher")]
        public ActionResult<string> AddTeacher(string courseid, string teacherid)
        {
            DBCode code = _access.CheckCoursePermission(courseid, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.AddTeacherToCourse(courseid, teacherid));
        }
        [HttpPost]
        [Route("Teacher/AddChecker")]
        public ActionResult<string> AddChecker(string courseid, string checkerid)
        {
             DBCode code = _access.CheckCoursePermission(courseid, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.AddCheckerToCourse(courseid, checkerid));
        }
        [HttpDelete]
        [Route("Teacher/DeleteChecker")]
        public ActionResult DeleteChecker(string courseid, string checkerid)
        {
             DBCode code = _access.CheckCoursePermission(courseid, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.DeleteCheckerFromCourse(courseid, checkerid));
        }
        [HttpGet]
        [Route("Teacher/GetTeachers")]
        public ActionResult<List<UserLabel>> GetTeachers(string courseid)
        {
            DBCode code = _access.CheckCoursePermission(courseid, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetCourseTeachers(courseid));
        }
        [HttpGet]
        [Route("Teacher/GetCheckers")]
        public ActionResult<List<UserLabel>> GetCourseCheckers(string courseid)
        {
            DBCode code = _access.CheckCoursePermission(courseid, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetCourseCheckers(courseid));
        }
        [HttpPost]
        [Route("Admin/AddCourse")]
        public ActionResult<string> AddCourse([FromBody] Course course)
        {
            if(!_access.IsAdmin)
            {
                return Forbid();
            }
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            course.Year =  (month >= 8) ? year + 1 : year;
            course.GenerateID();
            DBCode code = _access.AddCourse(course);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return course.ID;

        }
        [HttpPost]
        [Route("Teacher/AddStudent")]
        public ActionResult<string> AddStudent(string courseId, string studentId)
        {
            DBCode code = _access.CheckCoursePermission(courseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.AddCheckerToCourse(courseId, studentId));
        }
        [HttpPost]
        [Route("Teacher/AddStudents")]
        public ActionResult AddStudents(string courseId, [FromBody] string content)
        {
            DBCode code = _access.CheckCoursePermission(courseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            string path = FileUtils.GetTempFileName(".csv");
            System.IO.File.WriteAllText(path, content);
            content = null;
            ExcelLoader.AddStudentsToCourse(path, courseId);
            System.IO.File.Delete(path);
            return Ok();
        }
        [HttpPost]
        [Route("Admin/AddCourses")]
        public ActionResult AddCourses([FromBody] string content)
        {
            if(!_access.IsAdmin) { return Forbid(); }
            string path = FileUtils.GetTempFileName(".csv");
            System.IO.File.WriteAllText(path, content);
            content = null;
            ExcelLoader.ReadCoursesFromFile(path);
            System.IO.File.Delete(path);
            return Ok();
        }
    }
}

