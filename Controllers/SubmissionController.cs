using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace Submit_System.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SubmissionController : AbstractController
    {
        private readonly DatabaseAccess _access;
        private readonly MossClient _client;

        public SubmissionController(DatabaseAccess access, MossClient client)
        {
            _access = access;
            _client = client;
        }

        [Route("Student/GradesList")]
        [HttpGet]
        public ActionResult<List<ExerciseGradeDisplay>> GetStudentGrades(string userid)
        {
            return HandleDatabaseOutput(_access.GetStudentGrades(userid));
        }
        [Route("Student/SubmissionDetails")]
        [HttpGet]
        public ActionResult<StudentExInfo> GetSubmission(string userid, string exerciseId)
        {
            var a = _access.GetStudentSubmission(userid, exerciseId);
            return HandleDatabaseOutput(a);
        }
        [Route("Student/MessageList")]
        [HttpGet]
        public ActionResult<List<Message>> GetLateMessages(string userid, string chatId)
        {
            return HandleDatabaseOutput(_access.GetMesssages(userid, chatId, Role.Student));
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
            return HandleDatabaseOutput(_access.InsertStudentMessage(userid, chatId, msg));
        }
        [Route("Student/Appeal")]
        [HttpPost]        
        public ActionResult Appeal(string userid, string submissionid, [FromBody] MessageInput msg)
        {
            return HandleDatabaseOutput(_access.Appeal(userid, submissionid, msg));
        }
        [Route("Student/ExtensionRequest")]
        [HttpPost]        
        public ActionResult Extension(string userid, string submissionid, [FromBody] MessageInput msg)
        {
            return HandleDatabaseOutput(_access.Appeal(userid, submissionid, msg));
        }
        [Route("Student/SubmitExercise")]
        [HttpPost]        
        public ActionResult<SubmitResult> Submit(string userid, string exerciseId, [FromBody] List<SubmitFile> files, bool final)
        {
            if(!files?.Any() ?? false || exerciseId == null)
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
            (string path, string subid, DBCode code) =  _access.CreateSubmissionPath(userid, exerciseId);
            if(path == null) { return NotFound("Exercise not found"); }
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
                Debug.WriteLine(e.ToString());
                return ServerError("There was an issue with uploading the files");
            }
            if(final)
            {
                _access.MarkSubmitted(subid, userid);
            }
            return new SubmitResult {
                Message = $"Exercise Submitted successfully.\n Date Submitted: {DateTime.Now.ToString("m/dd/yyyy")}",
                Files = submittedFiles
            };
        }
        [HttpGet]
        [Route("Checker/SubmissionListToCheck")]
        public ActionResult<List<SubmissionLabel>> GetSubmissionsToCheck(string userid, string exerciseId)
        {
            return new StatusCodeResult(501);
        }
        [HttpGet]
        [Route("Checker/SubmissionToCheckList")]
        public ActionResult<List<Submission>> GetCheckedSubmissions(string userid, string exerciseId)
        {
            return new StatusCodeResult(501);
        }
        [HttpGet]
        [Route("Checker/SubmissionListToReCheck")]
        public ActionResult<List<Submission>> GetAppealSubmissions(string userid, string exerciseId)
        {
            return new StatusCodeResult(501);
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
        public ActionResult<SubmitFile> GetFile(string userid, string submissionId, string file)
        {
            (string submitDirectory, DBCode code) = _access.GetSubmissionDirectory(userid, submissionId, Role.Student);
            if(submitDirectory == null)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleFileSending(submitDirectory, file);
        }
        [HttpGet]
        [Route("Student/Download")]
        public ActionResult<SubmitFile> Download(string userid, string submissionId)
        {
            (string file, DBCode code) = _access.GetSubmissionDirectory(userid, submissionId, Role.Student);
            if(file == null)
            {
                return HandleDatabaseOutput(code);
            }
            var archiveBytes = FileUtils.ToArchiveBytes(file);
            if(archiveBytes == null)
            {
                return NotFound();
            }
            return SubmitFile.Create("ex.zip", archiveBytes);
        }
         [HttpPost]
        [Route("Teacher/RejectRequest")]
        public ActionResult<SubmitFile> CloseChat(string userid, string chatid)
        {
            if(chatid == null)
            {
                return BadRequest();
            }
            return HandleDatabaseOutput(_access.CloseChat(userid, chatid));
        }
        [HttpPost]
        [Route("Teacher/CopyCheck")]
        public ActionResult<string> CopyCheck(string userid, [FromBody] MossData data)
        {
            if(data == null)
            {
                return BadRequest();
            }
            (string folder, DBCode code) = _access.GetExerciseDirectory(data.ExerciseID, userid);
            string sub = Path.Combine(folder, SUBMISSIONS);
            string baseFiles = Path.Combine(folder, BASE_FILES);
            Directory.CreateDirectory(baseFiles);
            data.SubmissionsFolder =  sub;
            data.BaseFilesFolder =  baseFiles;
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            data.Result = _client.SendRequest(data);
            _access.AddMossCheck(data);
            return data.Result;
        }
    }
}