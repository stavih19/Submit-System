using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Submit_System.Controllers
{
    [ApiController]
    [Route("")]
    public class SubmissionController : ControllerBase
    {

        private readonly ILogger<ExerciseController> _logger;

        private readonly DatabaseAccess _access;

        public SubmissionController(ILogger<ExerciseController> logger)
        {
            _logger = logger;
            _access = new DatabaseAccess();
        }

        [Route("Student/GradesList")]
        [HttpGet]
        public ActionResult<List<ExerciseGradeDisplay>> GetStudentGrades(string token)
        {
            return _access.GetStudentGrades(token);
        }
        [Route("Student/SubmissionDetails")]
        [HttpGet]
        public ActionResult<StudentExInfo> GetSubmission(string token, string Sub) {
            return _access.GetStudentSubmission(token, "a");
        }
        [Route("Student/MessageList")]
        [HttpGet]
        public ActionResult<List<Message>> GetLateMessages(string token, string chatId) {
            return _access.GetMesssages(token, chatId);
        }

        [Route("Student/RunResult")]
        [HttpGet]
        public ActionResult<string> RunExercise(string token, string submitId) {
            return "this is the result";
        }
        [Route("Student/NewMessage")]
        [HttpPost]        
        public ActionResult PostMessage(string token, string chatId, [FromBody] string msg) {
            _access.InsertMessage(token, chatId, msg);
            return new OkResult();
        }
        [Route("Student/SubmitExercise")]
        [HttpPost]        
        public ActionResult Submit(string token, string exerciseId, [FromBody] SubmissionUpload upload) {
            if(upload == null) {
                return BadRequest();
            }
            _access.SubmitExercise(token, exerciseId, upload);
            return new OkResult();
        }
    }
}

