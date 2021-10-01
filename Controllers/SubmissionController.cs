using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;
using System.IO;
using System.Text;
using System.Net.Http;

namespace Submit_System.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SubmissionController : AbstractController
    {
        private readonly MossClient _client;

        public SubmissionController(DatabaseAccess access, MossClient client): base(access)
        {
            _client = client;
        }

        [Route("Student/GradesList")]
        [HttpGet]
        public ActionResult<List<ExerciseGradeDisplay>> GetStudentGrades()
        {
            return HandleDatabaseOutput(_access.GetStudentGrades());
        }
        [Route("Student/SubmissionDetails")]
        [HttpGet]
        public ActionResult<StudentExInfo> GetSubmission(string exerciseId)
        {
            return HandleDatabaseOutput(_access.GetStudentExerciseInfo(exerciseId));
        }
        [Route("Student/MessageList")]
        [HttpGet]
        public ActionResult<List<Message>> GetMessages(string chatId)
        {
            DBCode res = _access.CheckChatPerm(chatId, Role.Student);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            var a = _access.GetMesssages(chatId);
            return HandleDatabaseOutput(a);
        }
        [HttpGet]
        [Route("Student/MessageFile")]
        public ActionResult StudentGetMessageFile(int messageId)
        {
             DBCode res = _access.CheckMessagePerm(messageId, Role.Student);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            (string path, DBCode code) = DataBaseManager.GetMessageFile(messageId);
              if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            string type = FileUtils.GetMimeType(path);
            return File(System.IO.File.ReadAllBytes(path), type, FileUtils.GetFileName(path));
        }
        [HttpGet]
        [Route("Teacher/MessageFile")]
        public ActionResult TeacherGetMessageFile(int messageId)
        {
             DBCode res = _access.CheckMessagePerm(messageId, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            (string path, DBCode code) = DataBaseManager.GetMessageFile(messageId);
              if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            string type = FileUtils.GetMimeType(path);
            return File(System.IO.File.ReadAllBytes(path), type, FileUtils.GetFileName(path));
        }
        [Route("Student/RunResult")]
        [HttpGet]
        public ActionResult<SubmitOutput> RunExerciseS(string submitId)
        {
            DBCode res = _access.CheckSubmissionPerm(submitId, Role.Student);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            (Submission submission, DBCode code) = _access.GetSubmission(submitId);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            int type = ((int) submission.SubmissionStatus > (int) SubmissionState.Unchecked) ? 1 : 0;
            return AutoTest(submitId, type);
        }
        [Route("Checker/RunResult")]
        [HttpGet]
        public ActionResult<SubmitOutput> RunExerciseC(string submitId)
        {
            DBCode res = _access.CheckSubmissionPerm(submitId, Role.Checker);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            _access.BeginChecking(submitId);
            return AutoTest(submitId, 1);
        }
        [Route("Teacher/RunResult")]
        [HttpGet]
        public ActionResult<SubmitOutput> RunExerciseT(string submissionId)
        {
            DBCode res = _access.CheckSubmissionPerm(submissionId, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            _access.BeginChecking(submissionId);
            return AutoTest(submissionId, 1);
        }
        [Route("Student/NewMessage")]
        [HttpPost]        
        public ActionResult<int> PostStudentMessage(string chatId, [FromBody] MessageInput msg, [FromBody] string text)
        { 
            if(chatId == null)
            {
                return BadRequest();
            }
            if(msg?.Text == null & msg?.AttachedFile == null)
            {
                if(text == null)
                {
                    return BadRequest();
                }
                msg = new MessageInput {
                    Text = text,
                    ChatID = chatId
                };
            }
            msg.ChatID = chatId;
            DBCode res = _access.CheckChatPerm(msg.ChatID, Role.Student);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            return PostMessage(msg);
        }
        [NonAction]
        protected ActionResult<int> PostMessage(MessageInput msg)
        {
            if(msg?.ChatID == null || (msg?.Text == null && msg?.AttachedFile == null))
            {
                return BadRequest();
            }
            msg.Text = HttpUtility.HtmlEncode(msg.Text);
            string path = Path.Combine("Requests", msg.ChatID);
            // Create a randomly generated directory name. This prevents collisions if attached files have the same name
            if(!String.IsNullOrEmpty(msg?.AttachedFile?.Name) && !String.IsNullOrEmpty(msg?.AttachedFile?.Content))
            {
                path = FileUtils.CreateUniqueDirectory(path, "Attachment_");
                msg.FilePath = Path.Combine(path, msg.AttachedFile.Name);
                msg.AttachedFile.CreateFile(path);
            }
            (int id, DBCode code) = _access.InsertMessage(msg, false);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            
            return id;
        }
        [Route("Teacher/NewMessage")]
        [HttpPost]  
        public ActionResult<int> PostTeacherMessage(string chatId, [FromBody] MessageInput msg, [FromBody] string text)
        {
            if(chatId == null)
            {
                return BadRequest();
            }
            if(msg?.Text == null & msg?.AttachedFile == null)
            {
                if(text == null)
                {
                    return BadRequest();
                }
                msg = new MessageInput {
                    Text = text,
                    ChatID = chatId
                };
            }
            msg.ChatID = chatId;
            DBCode res = _access.CheckSubmissionPerm(msg.ChatID, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            return PostMessage(msg);
        }
        [Route("Student/Appeal")]
        [HttpPost]        
        public ActionResult<string> Appeal(string submissionid, [FromBody] MessageInput msg, [FromBody] string text)
        {
            DBCode res = _access.CheckSubmissionPerm(submissionid, Role.Student);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            if(msg == null)
            {
                msg = new MessageInput { Text = text };
            }
            (string id, DBCode code) = _access.Appeal(submissionid);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            msg.ChatID = id;
            PostMessage(msg);
            return id;
    
        }
        [Route("Student/ExtensionRequest")]
        [HttpPost]        
        public ActionResult<string> Extension(string submissionid, [FromBody] MessageInput msg, [FromBody] string text)
        {
            DBCode res = _access.CheckSubmissionPerm(submissionid, Role.Student);
            if(msg == null)
            {
                msg = new MessageInput { Text = text };
            }
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            (string id, DBCode code) = _access.ExtensionRequest(submissionid);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            msg.ChatID = id;
            PostMessage(msg);
            return id;
        }
        public void SendResults(SubmitInfo info, List<User> submitters) 
        {
            foreach(User submitter in submitters)
            {
                string subject = $"Course {info.CourseID} Exercise {info.ExerciseName}";
                MaleUtils.SendMail(submitter.Email, subject, info.Text);
            }
        }
        [Route("Student/DetachFromSubmission")]
        [HttpPost]        
        public ActionResult<SubmitResult> Detach(string submissionId)
        {
            return HandleDatabaseOutput(_access.DetachStudent(submissionId));
        }   
        [Route("Student/CancelSubmission")]
        [HttpPost]        
        public ActionResult<SubmitResult> CancelSubmission(string submissionId)
        {
            return HandleDatabaseOutput(_access.CancelSubmission(submissionId));
        }       
        [Route("Student/SubmitExercise")]
        [HttpPost]        
        public ActionResult<SubmitResult> Submit(string exerciseId, [FromBody] List<SubmitFile> files, bool final)
        {
            if(!(files?.Any() ?? false) || exerciseId == null)
            {
                return BadRequest("No files");
            }
            foreach(var file in files)
            {
                if(IsAnyNull(file?.Content, file?.Name))
                {
                    return BadRequest("No files");
                }
            }
            DBCode res = _access.CheckExercisePermission(exerciseId, Role.Student);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            (SubmitInfo info, DBCode code) =  _access.GetSubmitInfo(exerciseId);
            if(info?.Path == null) { return NotFound("Exercise not found"); }
            List<string> submittedFiles;
            try
            {
                submittedFiles = FileUtils.StoreFiles(files, info.Path, true, true);
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
            Result result;
            int styleGrade;
            try
            {   
                result = TestManager.getTestResults(info.SubmissionID, 0);
                styleGrade = result.Check_Style_Result.IsOk ? 100 : 0;
            }
            catch
            {
                return new SubmitResult {
                    Message = "Programming langauge not supported by automatic tester",
                    Files = submittedFiles,
                    AutoGrade = -1,
                    StyleGrade = -1
                };
            }
            if(final)
            {
                _access.MarkSubmitted(info.SubmissionID, styleGrade);
            }
            return new SubmitResult {
                Message = result.ToHTML(),
                Files = submittedFiles,
                AutoGrade = (int) Math.Round(result.GetTestsGrade()),
                StyleGrade = styleGrade

            };
        }
        [HttpGet]
        [Route("Checker/SubmissionLabels")]
        public ActionResult<Dictionary<string, List<SubmissionLabel>>> CheckerGetSubmissionsToCheck(string exerciseId)
        {
            DBCode res = _access.CheckExercisePermission(exerciseId, Role.Checker);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            return HandleDatabaseOutput(_access.GetSubmissionLabels(exerciseId));
        }
        [HttpGet]
        [Route("Teacher/SubmissionLabels")]
        public ActionResult<Dictionary<string, List<SubmissionLabel>>> GetSubmissionsToCheck(string exerciseId)
        {
            DBCode res = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            return HandleDatabaseOutput(_access.GetSubmissionLabels(exerciseId));
        }
        [HttpGet]
        [Route("Teacher/GetSubmission")]
        public ActionResult<Submission> GetSubmissionToCheck(string submissionId)
        {
            DBCode code = _access.CheckSubmissionPerm(submissionId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetSubmission(submissionId));
        }
        [HttpGet]
        [Route("Checker/GetSubmission")]
        public ActionResult<Submission> CheckerGetSubmissionToCheck(string submissionId)
        {
            DBCode code = _access.CheckSubmissionPerm(submissionId, Role.Checker);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetSubmission(submissionId));
        }
        [HttpGet]
        [Route("Teacher/BeginChecking")]
        public ActionResult<Submission> TeacherBeginCheck(string submissionId)
        {
            DBCode code = _access.CheckSubmissionPerm(submissionId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.BeginChecking(submissionId));
        }
        [HttpGet]
        [Route("Checker/BeginChecking")]
        public ActionResult<Submission> CheckerBeginCheck(string submissionId)
        {
            DBCode code = _access.CheckSubmissionPerm(submissionId, Role.Checker);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.BeginChecking(submissionId));
        }
        [HttpGet]
        [Route("Checker/EndChecking")]
        public ActionResult<Submission> CheckerEndCheck([FromBody] Submission submission)
        {
            DBCode code = _access.CheckSubmissionPerm(submission.ID, Role.Checker);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.EndChecking(submission));
        }
        [HttpGet]
        [Route("Teacher/EndChecking")]
        public ActionResult<Submission> TeacherEndCheck([FromBody] Submission submission)
        {
            DBCode code = _access.CheckSubmissionPerm(submission.ID, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.EndChecking(submission));
        }
        [HttpPost]
        [Route("Student/ValidateSubmitters")]
        public ActionResult<Dictionary<string, bool>> Validate(string exerciseId, [FromBody] List<string> submitters)
        {
            return HandleDatabaseOutput(_access.ValidateSubmitters(exerciseId, submitters));
        }
        [HttpGet]
        [Route("Teacher/GetFile")]
        public ActionResult TeacherGetFile(string submissionId, string file)
        {
            return GetFile(submissionId, file, Role.Teacher);
        }
        [HttpGet]
        [Route("Checker/GetFile")]
        public ActionResult CheckerGetFile(string submissionId, string file)
        {
            return GetFile(submissionId, file, Role.Checker);
        }
        [HttpGet]
        [Route("Student/GetFile")]
        public ActionResult StudentGetFile(string submissionId, string file)
        {
            return GetFile(submissionId, file, Role.Student);
        }
        [NonAction]
        private ActionResult GetFile(string submissionId, string file, Role role)
        {
            DBCode res = _access.CheckSubmissionPerm(submissionId, role);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            (string submitDirectory, DBCode code) = _access.GetSubmissionDirectory(submissionId);
            if(submitDirectory == null)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleFileSending(submitDirectory, file);
        }
        [HttpGet]
        [Route("Student/Download")]
        public ActionResult Download(string submissionId)
        {
            DBCode res = _access.CheckSubmissionPerm(submissionId, Role.Student);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            (string dir, DBCode code) = _access.GetSubmissionDirectory(submissionId);
            if(dir == null)
            {
                return NotFound();
            }
            return HandleArchiveSending(dir, "ex.zip");
        }
        [HttpPost]
        [Route("Teacher/RejectRequest")]
        public ActionResult<SubmitFile> CloseChat(string chatid)
        {
            if(chatid == null)
            {
                return BadRequest();
            }
            DBCode res = _access.CheckChatPerm(chatid, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            return HandleDatabaseOutput(_access.CloseChat(chatid));
        }
        [HttpPost]
        [Route("Teacher/AcceptAppeal")]
        public ActionResult<SubmitFile> AcceptAppeal(string chatid)
        {
            if(chatid == null)
            {
                return BadRequest();
            }
            DBCode res = _access.CheckChatPerm(chatid, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            return HandleDatabaseOutput(_access.AcceptAppeal(chatid));
        }
        [HttpPost]
        [Route("Teacher/AcceptExtension")]
        public ActionResult<SubmitFile> Extend(string chatid, [FromBody] SubmitDate date)
        {
            if(chatid == null)
            {
                return BadRequest();
            }
            DBCode res = _access.CheckChatPerm(chatid, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            return HandleDatabaseOutput(_access.AcceptExtension(chatid, date));
        }
        [HttpPost]
        [Route("Teacher/CopyCheck")]
        public ActionResult<string> CopyCheck([FromBody] MossData data)
        {
            if(data?.ExerciseID == null)
            {
                return BadRequest("no exercise");
            }
            DBCode res = _access.CheckExercisePermission(data.ExerciseID, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            (Exercise ex, DBCode code) = _access.GetExercise(data.ExerciseID);
            data.Language = ex.ProgrammingLanguage;
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            if(data.Language.StartsWith("python"))
            {
                data.Language = "python";
            }
            if(!MossClient.IsSupported(data.Language))
            {
                var result = new ObjectResult($"The language {data.Language} is not supported.");
                result.StatusCode = 501;
                return result;
            }
            string folder = ex.FilesLocation;
            string sub = Path.Combine(folder, SUBMISSIONS);
            Directory.CreateDirectory(sub);
            string baseFiles = Path.Combine(folder, BASE_FILES);
            Directory.CreateDirectory(baseFiles);
            data.SubmissionsFolder =  sub;
            data.BaseFilesFolder =  baseFiles;
            try
            {
                data.Result = _client.SendRequest(data);
            }
            catch(Exception)
            {
                return new StatusCodeResult(502);
            }
            _access.AddMossCheck(data);
            return data.Result;
        }
        [HttpPost]
        [Route("Teacher/SetCopied")]
        public ActionResult MarkCopy([FromBody] CopyForm copy)
        {
            DBCode res = _access.CheckExercisePermission(copy.ExerciseID, Role.Teacher);
            if(res != DBCode.OK)
            {
                return HandleDatabaseOutput(res);
            }
            if(IsAnyNull(copy?.User1, copy?.User2, copy?.ExerciseID))
            {
                return BadRequest("fields are null");
            }
            return HandleDatabaseOutput(_access.MarkCopy(copy));
        }
        [HttpGet]
        [Route("Teacher/GetAppeals")]
        public ActionResult<List<RequestLabel>> Appeals(string exerciseId)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetRequests(exerciseId, ChatType.Appeal));
        }
        [HttpGet]
        [Route("Teacher/GetExtensions")]
        public ActionResult<List<RequestLabel>> Extensions(string exerciseId)
        {
            DBCode code = _access.CheckExercisePermission(exerciseId, Role.Teacher);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return HandleDatabaseOutput(_access.GetRequests(exerciseId, ChatType.Extension));
        }
        [HttpGet]
        [Route("Teacher/GetTeacherAppeals")]
        public ActionResult<List<RequestLabelMainPage>> Appeals()
        {
            return HandleDatabaseOutput(_access.GetRequests(ChatType.Appeal));
        }
        [HttpGet]
        [Route("Teacher/GetTeacherExtensions")]
        public ActionResult<List<RequestLabelMainPage>> Extensions()
        {
            return HandleDatabaseOutput(_access.GetRequests(ChatType.Extension));
        }
        [NonAction]
        private ActionResult<SubmitOutput> AutoTest(string submissionId, int type)
        {
            Result result;
            try
            {
                result = TestManager.getTestResults(submissionId, type);
            }
            catch(NotImplementedException)
            {
                return null;
            }
            if(result == null)
            {
                return ServerError();
            }
            return new SubmitOutput {
                Text = result.ToHTML(),
                StyleGrade = result.Check_Style_Result.IsOk ? 100 : 0,
                AutoGrade = (int) Math.Round(result.GetTestsGrade())
            };
        }
    }
}