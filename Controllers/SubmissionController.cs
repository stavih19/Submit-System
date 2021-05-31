using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;

namespace Submit_System.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SubmissionController : AbstractController
    {

        private readonly ILogger<ExerciseController> _logger;

        private readonly FakeDatabaseAccess _access;

        public SubmissionController(ILogger<ExerciseController> logger, FakeDatabaseAccess access)
        {
            _logger = logger;
            _access = access;
        }

        [Route("Student/GradesList")]
        [HttpGet]
        public ActionResult<List<ExerciseGradeDisplay>> GetStudentGrades(string userid)
        {
            return _access.GetStudentGrades(userid);
        }
        [Route("Student/SubmissionDetails")]
        [HttpGet]
        public ActionResult<StudentExInfo> GetSubmission(string userid, string exerciseId)
        {
            var a = _access.GetStudentSubmission(userid, exerciseId);
            if(a == null)
            {
                return NotFound("Exercise Not found");
;           }
            return a;
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
            return "Exercise Submitted successfully.";
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
        public ActionResult<SubmitResult> Submit(string userid, string exerciseId, [FromBody] List<SubmitFile> files)
        {
            if(files == null) {
                return BadRequest("No files");
            }
            if(!files.Any())
            {
                return BadRequest("No files");
            }
            foreach(var file in files)
            {
                if(file == null || file?.Name == null || file?.Content == null)
                {
                    return BadRequest("No files");
                }
            }
            if(exerciseId == null || exerciseId == "")
            {
                return BadRequest("No exercise ID");
            }
            string path = _access.CreateDirectoryPath(userid, exerciseId);
            if(path == null)
            {
                return NotFound("Exercise not found");
            }
            List<string> submittedFiles;
            try
            {
                submittedFiles = FileUtils.SubmitFiles(files, path);
            }
            catch(System.Security.SecurityException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return ServerError("There was an issue with uploading the files");
            }
            return new SubmitResult {
                Message = $"Exercise Submitted successfully.\n Date Submitted: {DateTime.Now.ToString("m/dd/yyyy")}",
                Files = submittedFiles
            };
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

        [HttpPost]
        [Route("Student/ValidateSubmitters")]
        public ActionResult<Dictionary<string, bool>> Validate(string userid, string exerciseId, [FromBody] List<string> studentIdList)
        {
            var result =  new Dictionary<string, bool>();
            foreach(var id in studentIdList)
            {
                result[id] = true;
            }
            return result;
        }
        [HttpGet]
        [Route("Student/GetFile")]
        public ActionResult<SubmitFile> GetFile(string userid, string submissionId, [FromBody] string file)
        {
            // string ct;
            string submitDirectory = _access.GetDirectory(userid, submissionId);
            if(submitDirectory == null)
            {
                return NotFound("Submisison not found");
            }
            string fullPath = FileUtils.GetFullPath(file, submitDirectory);
            byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
            return SubmitFile.Create(file, bytes);
        }
        [HttpGet]
        [Route("Student/Download")]
        public ActionResult<SubmitFile> Download(string userid, string submissionId)
        {
            string file = _access.GetDirectory(userid, submissionId);
            if(file == null)
            {
                return NotFound("Submission not found");
            }
            var archiveBytes = FileUtils.ToArchiveBytes(file);
            return SubmitFile.Create("ex.zip", archiveBytes);
        }
    }
}

