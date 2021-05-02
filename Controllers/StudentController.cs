using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Submit_System.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<StudentController> _logger;

        private readonly DatabaseAccess _access;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
            _access = new DatabaseAccess();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Route("courseList")]
        [HttpGet]
        public ActionResult<List<Course>> CourseList(string token)
        { 
            return _access.GetCourses(token, CourseRole.Student);
        }
        [HttpGet]
        public ActionResult<List<ExerciseLabel>> ExerciseList(string token, string courseId)
        {
            return _access.GetCourseExercises(courseId);
        }
        
        [HttpGet]
        public ActionResult<List<ExerciseGradeDisplay>> GradesList(string token)
        {
            return _access.GetStudentGrades(token);
        }
        [HttpGet]
        public ActionResult<List<ExerciseDateDisplay>> ExerciseDates(string token)
        {
            return _access.GetExcercisesDates(token);
        }
    }
}

