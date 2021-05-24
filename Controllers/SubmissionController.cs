using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace Submit_System.Controllers
{
    [ApiController]
    [Route("")]
    [ServiceFilter(typeof(AuthFilter))]
    public class SubmissionController : AbstractController
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
        public ActionResult<List<ExerciseGradeDisplay>> GetStudentGrades(string userid)
        {
            return _access.GetStudentGrades(userid);
        }
        [Route("Student/SubmissionDetails")]
        [HttpGet]
        public ActionResult<StudentExInfo> GetSubmission(string userid, string Sub)
        {
            return _access.GetStudentSubmission(userid, "a");
        }
        [Route("Student/MessageList")]
        [HttpGet]
        public ActionResult<List<Message>> GetLateMessages(string userid, string chatId)
        {
            return _access.GetMesssages(userid, chatId);
        }

        [Route("Student/RunResult")]
        [HttpGet]
        public ActionResult<string> RunExercise(string userid, string submitId)
        {
            return "this is the result";
        }
        [Route("Student/NewMessage")]
        [HttpPost]        
        public ActionResult PostMessage(string userid, string chatId, [FromBody] string msg)
        {
            _access.InsertMessage(userid, chatId, msg);
            return new OkResult();
        }
        [Route("Student/SubmitExercise")]
        [HttpPost]        
        public IActionResult Submit(string token, string exerciseId, [FromBody]SubmissionUpload upload)
        {
            if(upload == null || upload?.Files == null) {
                return BadRequest("No files");
            }
            if(exerciseId == null || exerciseId == "")
            {
                return BadRequest("No exercise ID");
            }
            string path = $"./Exercises/{exerciseId}/Submissions/{token}";
            try
            {
                //FileUtils.StoreFiles(path, upload.Files);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return ReturnResult(HttpStatusCode.InternalServerError, "There was an issue with uploading the files");
            }
            // string path = _access.CreateSubmissionPath();
            return Ok();
        }
        [HttpGet]
        [Route("Checker/SubmissionListToCheck")]
        public ActionResult<List<SubmissionData>> GetSubmissionsToCheck(string userid, string exerciseId)
        {
            return FakeDatabase.Submissions;
        }
        [HttpGet]
        [Route("Checker/SubmissionToCheckList")]
        public ActionResult<List<SubmissionData>> GetCheckedSubmissions(string userid, string exerciseId)
        {
            return FakeDatabase.Submissions;
        }
        [HttpGet]
        [Route("Checker/SubmissionListToReCheck")]
        public ActionResult<List<SubmissionData>> GetAppealSubmissions(string userid, string exerciseId)
        {
            return FakeDatabase.Submissions;
        }
    }
}

