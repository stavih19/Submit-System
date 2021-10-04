using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
namespace Submit_System {
    public enum DBCode { OK, NotFound, Invalid, Error, NotAllowed, AlreadyExists }
    public enum Role { Student, Checker, Teacher, None }
    public class DatabaseAccess {
        private readonly IMemoryCache _cache;

        private UserPermissions _userPerms;
        public string UserID { get; private set; } = "N/A";

        public bool IsAdmin {get; private set; }
        public string ErrorString { get; private set; }
        public DatabaseAccess(IMemoryCache _userPermsCache)
        {
            _cache = _userPermsCache;
        }
        public void SetUser(string userId, bool isAdmin)
        {
            UserID = userId;
            _userPerms = _cache.GetOrCreate(("UserPerm" + userId), entry => {
                entry.SlidingExpiration = TimeSpan.FromMinutes(75);
                entry.SetSize(1);
                return new UserPermissions();
            });
            IsAdmin = isAdmin;
        }
        private (T, DBCode) Error<T>(string err, DBCode code)
        {
            ErrorString = err;
            Trace.WriteLine(err);
            return (default(T), code);
        }
        private void Log(string msg)
        {
            Debug.WriteLine(msg);
        }
        private (T, DBCode) Convert<T>((T obj, int code, string msg) output)
        {
            if(CheckError(output.code))
            {
                Log(output.msg);
            }
            DBCode newCode = (output.code == 4) ?  DBCode.OK : (DBCode) output.code;
            return (output.obj, newCode);
        }
        private DBCode Convert((int code, string msg) output)
        {
            if(CheckError(output.code))
            {
                Log(output.msg);
                ErrorString = output.msg;
            }
            return (DBCode) output.code;
        }
        private void CacheSubmissionDirectory(string submissionId, string folder)
        {
            var options = new MemoryCacheEntryOptions { Size = 1 };
            _cache.Set("SubDir" + submissionId, folder, options); 
        }
        private void CacheExerciseDirectory(string exerciseId, string folder)
        {
            var options = new MemoryCacheEntryOptions { Size = 1 };
            _cache.Set("ExDir" + exerciseId, folder, options); 
        }
        public DBCode CheckCoursePermission(string courseid, Role role)
        {
            bool has = false;
            if(_userPerms == null)
            {
                return DBCode.NotAllowed;
            }
            Monitor.Enter(_userPerms);
            has = _userPerms.CheckCoursePerm(courseid, role);
            Monitor.Exit(_userPerms);
            if(has) return DBCode.OK;
            // TODO do a real prmission check
            if(role == Role.Teacher) return DBCode.OK;
            DBCode code = CheckResourcePerms(courseid, role, ResourceType.Course);
            if(code == DBCode.OK)
            {
                AddCoursePerms(courseid, Role.Student);
            }
            return code;
        }
        public void AddCoursePerms(string courseid, Role role)
        {
            AddCoursePerms(new List<string>{courseid}, Role.Student);
        }

        public void AddCoursePerms(List<string> courseids, Role role)
        {
            Monitor.Enter(_userPerms);
            foreach(var id in courseids)
            {
                _userPerms.SetCoursePerm(id, role);
            }
            Monitor.Exit(_userPerms);
        }
        public void AddExercisePerm(string exerciseid, Role role)
        {
            Monitor.Enter(_userPerms);
            _userPerms.SetExercisePerm(exerciseid, role);
            Monitor.Exit(_userPerms);
        }
        public (string, DBCode) GetExerciseDirectory(string exerciseID)
        {
            string folder;
            if(!_cache.TryGetValue("ExDir" + exerciseID, out folder))
            {
                (string path, DBCode code) = DataBaseManager.GetExDir(exerciseID);
                if(CheckError(code))
                {
                    return (null, code);
                }
                CacheExerciseDirectory(exerciseID, path);
                return (path, DBCode.OK);
            }
            return (folder, DBCode.OK);

        }
        public DBCode CheckExercisePermission(string exerciseid, Role role)
        {
            if(_userPerms == null)
            {
                return DBCode.NotAllowed;
            }
            Monitor.Enter(_userPerms);
            bool has = _userPerms.CheckExercisePerm(exerciseid, role);
            Monitor.Exit(_userPerms);
            if(has)
            {
                return DBCode.OK;
            }
            DBCode code = CheckResourcePerms(exerciseid,role, ResourceType.Exercise);
            if(code != DBCode.OK && role == Role.Teacher)
            {
                code = CheckResourcePerms(exerciseid, role, ResourceType.OldExercise);
            }
            if(code == DBCode.OK)
            {
                AddExercisePerm(exerciseid, role);
            }
            return code;
        }
        public DBCode CheckSubmissionPerm(string submissionid, Role role)
        {
            if(_userPerms == null)
            {
                return DBCode.NotAllowed;
            }
            Monitor.Enter(_userPerms);
            bool has = _userPerms.CheckSubmissionPerm(submissionid, role);
            Monitor.Exit(_userPerms);
            if(has)
            {
                return DBCode.OK;
            }
            return CheckResourcePerms(submissionid,role, ResourceType.Submission);
        }
        public DBCode CheckOtherStudentExercisePermission(string studentId, string exerciseid)
        {
            return DataBaseManager.CheckResourcePerms(exerciseid, studentId, Role.Student, ResourceType.Exercise);
        }
         public void AddSubmissionPerm(string submissionid, Role role)
        {
            Monitor.Enter(_userPerms);
            _userPerms.SetSubmissionPerm(submissionid, role);
            Monitor.Exit(_userPerms);
        }
        public DBCode CheckChatPerm(string chatid, Role role)
        {
            if(_userPerms == null)
            {
                return DBCode.NotAllowed;
            }
            Monitor.Enter(_userPerms);
            bool has = _userPerms.CheckSubmissionPerm(chatid, role);
            Monitor.Exit(_userPerms);
            if(has)
            {
                return DBCode.OK;
            }
            return CheckResourcePerms(chatid, role, ResourceType.Chat);
        }
        public void AddChatPerm(string chatid, Role role, bool isExt)
        {
            Monitor.Enter(_userPerms);
            _userPerms.SetChatPerm(chatid, role, isExt);
            Monitor.Exit(_userPerms);
        }
        public DBCode CheckMessagePerm(int messageId, Role role)
        {
            return CheckResourcePerms(messageId, role, ResourceType.Message);
        }
        public bool CheckError(DBCode code)
        {
            return CheckError((int) code);
        }
        public bool CheckError(int code)
        {
            return code != 0 && code != 4;
        }
        public (string, DBCode) AuthenticateUser(string id, string password, out bool isAdmin)
        {
            isAdmin = false;
            (User user, DBCode code) = Convert(DataBaseManager.ReadUser(id));
            if(CheckError(code))
            {
                return (null, code);
            }
            if(!CryptoUtils.KDFVerify(password, user.PasswordHash))
            {
                return (null, DBCode.NotFound);
            }
            (isAdmin, code) = DataBaseManager.IsAdmin(id);
            return (user.Name, code);
        }
        public (string, DBCode) GetSubmissionDirectory(string submisisonId)
        {
            string folder;
            if(!_cache.TryGetValue("SubDir" + submisisonId, out folder))
            {
                (string path, DBCode code2) = DataBaseManager.GetSubDir(submisisonId);
                if(CheckError(code2))
                {
                    return (null, code2);
                }
                CacheExerciseDirectory(submisisonId, path);
                folder = path;
            }
            return (folder, DBCode.OK);
        }
        public (SubmitInfo, DBCode) GetSubmitInfo(string exerciseId)
        {
           (Exercise exer, DBCode exCode) = Convert(DataBaseManager.ReadExercise(exerciseId));
           if(CheckError(exCode))
           {
               return (null, (DBCode) exCode);
           }
           ((var submission, int type), DBCode code) = GetStudentSubmission(UserID, exerciseId);
           if(type != 1)
           {
               ErrorString = "You are not the main submitter";
               return (null, DBCode.NotAllowed);
           }
           if(submission.SubmissionStatus != (int) SubmissionState.Unsubmitted && !exer.MultipleSubmission)
           {
               ErrorString = "Resubmission is not allowed for this assignment";
               return (null, DBCode.NotAllowed);
           }
           (var dates, DBCode code3) = DataBaseManager.GetStudentExerciseDates(submission.SubmissionDateId, exerciseId);
           bool canSubmit = dates.Select((date) => date.Date.AddDays(exer.Reductions.Length) > DateTime.Now.Date).Any();
           if(!canSubmit)
           {
                ErrorString = "It's too late for you to submit this";
                return (null, DBCode.NotAllowed);
           }
           CacheSubmissionDirectory(submission.ID, submission.FilesLocation);
            SubmitInfo info = new SubmitInfo {
                CourseID = exer.CourseID,
                ExerciseName = exer.ID,
                SubmissionID = submission.ID,
                Path = submission.FilesLocation,
                ExerciseID = exerciseId
            };
           return (info, DBCode.OK);

        }
        public (List<Course>, DBCode) GetCourses(Role role) {
            (List<Course> courseList, DBCode code) output = DataBaseManager.GetCoursesOfUser(UserID, role);
            if(output.code != DBCode.OK)
            {
                return output;
            }
            List<string> ids = output.courseList.Select((Course course) => course.ID).ToList();
            AddCoursePerms(ids, role);
            return output;
        }
        public (List<ExerciseLabel>, DBCode) GetCourseExercises(string courseId) {
            
            return Convert(DataBaseManager.ReadExerciseLabelsOfCourse(courseId));
        }

        public (List<CheckerExInfo>, DBCode) GetCheckerExercises()
        {
          return DataBaseManager.CheckerExercises(UserID);
        }
        public (List<ExerciseGradeDisplay>, DBCode) GetStudentGrades()
        {
            return DataBaseManager.GetExerciseGrades(UserID);
        }

        public (SortedDictionary<string, List<ExerciseLabel>>, DBCode) GetAllExercises(string courseid)
        {
            return Convert(DataBaseManager.GetAllExercises(courseid));
        }

        public (List<UserLabel>, DBCode) GetCourseTeachers(string courseid)
        {
            return DataBaseManager.GetUsersOfCourse(courseid, Role.Teacher);
        }
        public (List<UserLabel>, DBCode) GetCourseCheckers(string courseid)
        {
            return DataBaseManager.GetUsersOfCourse(courseid, Role.Checker);
        }
        public (Submission, DBCode) CreateSubmission(string exId)
        {
            (string path, DBCode code) = DataBaseManager.GetExDir(exId);
            if(path == null)
            {
                return (null, code);
            }
            var s = new Submission {
                ID = Submission.GenerateID(UserID, exId),
                ExerciseID = exId,
                SubmissionStatus = (int) SubmissionState.Unsubmitted,
                AutoGrade = -1,
                StyleGrade = -1,
                ManualFinalGrade = -1,
                ManualCheckData = null,
                TimeSubmitted = DateTime.Now,
                SubmissionDateId = -1,
                FilesLocation = Path.Combine(path, "Submissions", UserID)
            };
            DataBaseManager.AddStudentToSubmission(s.ID, UserID, 1, exId);
            DataBaseManager.AddSubmission(s);
            return (s, DBCode.OK);
        }
        private DBCode CheckResourcePerms(string resourceId, Role role, ResourceType resource)
        {
            if(IsAdmin)
            {
                return DBCode.OK;
            }
            return DataBaseManager.CheckResourcePerms(resourceId, UserID, role, resource);
        }
        private DBCode CheckResourcePerms(int resourceId, Role role, ResourceType resource)
        {
            if(IsAdmin)
            {
                return DBCode.OK;
            }
            return DataBaseManager.CheckResourcePerms(resourceId, UserID, role, resource);
        }
        public DBCode CheckTeacherDatePerm(int dateid)
        {
            return CheckResourcePerms(dateid, Role.Teacher, ResourceType.Date);
        }
        public DBCode CheckTestPerm(int testid, Role role)
        {
            if(IsAdmin)
            {
                return DBCode.OK;
            }
            DBCode code = CheckResourcePerms(testid, role, ResourceType.Test);
            if(code != DBCode.OK && role == Role.Teacher)
            {
                code = CheckResourcePerms(testid, Role.Teacher, ResourceType.OldTest);
            }
            return code;
        }
        public (List<ExerciseDateDisplay>, DBCode) GetStudentExcercisesDates() {
           var output = DataBaseManager.GetStudentExerciseDates(UserID);
           return output;
        }
        public ((Submission, int), DBCode) GetStudentSubmission(string userID, string exerciseId)
        {
            ((string id, int type), DBCode code) = Convert(DataBaseManager.ReadSubmissionIdAndTypeOfStudentInExercise(UserID, exerciseId));
            if(code == DBCode.NotFound)
            {
                code = CheckExercisePermission(exerciseId, Role.Student);
                if(code != DBCode.OK)
                {
                    return ((null, -1), code);
                }
                var a = CreateSubmission(exerciseId);
                return ((a.Item1, 1), a.Item2);
            }
            else if(code == DBCode.Error)
            {
                return ((null, -1), DBCode.Error);
            }
            (Submission submission, DBCode code2) = Convert(DataBaseManager.ReadSubmission(id));
            if(code2 == DBCode.NotFound)
            {
                DataBaseManager.DeleteStudentFromSubmission(userID, exerciseId);
                (submission, code2) = CreateSubmission(exerciseId);
                return ((submission, 1), code2);
            }
            else if(code2 == DBCode.Error)
            {
                return ((null, -1), DBCode.Error);
            }
            return ((submission, type), code2);
        }
        public (StudentExInfo, DBCode) GetStudentExerciseInfo(string exId)
        {
            (Exercise ex, DBCode code) = Convert(DataBaseManager.ReadExercise(exId));
            if(code != DBCode.OK)
            {
                return (null, code);
            }
            ((var submission, int type), DBCode code2) = GetStudentSubmission(UserID, exId);
            if(CheckError(code2))
            { 
                return (null, code2);
            }
            AddSubmissionPerm(submission.ID, Role.Student);
            (var chats, DBCode code3) = Convert(DataBaseManager.ReadChatsOfSubmission(submission.ID));
            if(CheckError(code3))
            { 
                return (null, code3);
            }
            if(chats[ChatType.Extension] != null)
            {
                AddChatPerm(chats[ChatType.Extension].ID, Role.Student, true);
            }
             if(chats[ChatType.Appeal] != null)
            {
                AddChatPerm(chats[ChatType.Appeal].ID, Role.Student, false);
            }
            (var submitters, DBCode code4) = DataBaseManager.GetSubmitters(submission.ID);
            if(CheckError(code4))
            { 
                return (null, code4);
            }
            (var dates, DBCode code5) = DataBaseManager.GetStudentExerciseDates(submission.SubmissionDateId, exId);
            if(CheckError(code5))
            {
                return (null, code5);
            }
            int r = 0;
            foreach(SubmitDate date in dates)
            {
                if(date.Date.Date >= DateTime.Now.Date)
                {
                    r = date.Reduction;
                    break;
                }
            }
            if(CheckError(code5))
            { 
                return (null, code5);
            }
            StudentExInfo info = new StudentExInfo {
                SubmissionID = submission.ID,
                ExID = ex.ID,
                ExName = ex.Name,
                DateSubmitted = submission.TimeSubmitted,
                AutoWeight = ex.AutoTestGradeWeight,
                StyleWeight = ex.StyleTestGradeWeight,
                AutoGrade = submission.AutoGrade,
                StyleGrade = submission.StyleGrade,
                ManualGrade = submission.ManualFinalGrade,
                SubmissionFolder = submission.FilesLocation,
                Filenames = FileUtils.GetRelativePaths(submission.FilesLocation),
                ExtensionChat = chats[ChatType.Extension],
                Reductions = ex.Reductions,
                AppealChat = chats[ChatType.Appeal],
                MaxSubmitters = ex.MaxSubmitters,
                Dates = dates,
                ManualCheckInfo = submission.ManualCheckData,
                IsMultipleSubmission = ex.MultipleSubmission,
                State = (SubmissionState) submission.SubmissionStatus,
                InitReduction = r,
                HasCopied = submission.HasCopied,
                Submitters = submitters,
                IsMainSubmitter = (type == 1)
            };
            return (info, DBCode.OK);
        }
        public DBCode CreateExercise(ExerciseInput input)
        {
            var exercise = input.Exercise;
            DBCode code =  Convert(DataBaseManager.AddExercise(exercise));
            if(CheckError(code) || input.MainDate == null)
            {
                return code;
            }
            var date = new SubmitDate {
                Group = 0,
                Reduction = 0,
                Date = (DateTime) input.MainDate,
                ExerciseID = exercise.ID
            };
            return AddDate(date).Item2;

        }
        public (List<Message>, DBCode) GetMesssages(string chatId)
        {
            return Convert(DataBaseManager.ReadMessagesOfChat(chatId));
        }
        public (string, DBCode) Appeal(string submissionId)
        {
            (var sub, DBCode code) = Convert(DataBaseManager.ReadSubmission(submissionId));
            var state = (SubmissionState) sub.SubmissionStatus;
            if(state != SubmissionState.Checked)
            {
                return Error<string>("Either this exericse was already appealed or it wasn't checked yet", DBCode.NotAllowed);
            }
            return AddRequestChat(submissionId, ChatType.Appeal);
        }
        public (string, DBCode) ExtensionRequest(string submissionId)
        {
            (var sub, DBCode code) = Convert(DataBaseManager.ReadSubmission(submissionId));
            if(code != DBCode.OK)
            {
                return (null, code);
            }
            var state = (SubmissionState) sub.SubmissionStatus;
            if(state != SubmissionState.Unchecked && state != SubmissionState.Unsubmitted)
            {
                return Error<string>("This submission was already checked", DBCode.NotAllowed);
            }
            return AddRequestChat(submissionId, ChatType.Extension);
        }
        private (string, DBCode) AddRequestChat(string submissionId, ChatType type)
        {
            var chat = new Chat(submissionId, type);
            DBCode code = Convert(DataBaseManager.AddChat(chat));
            bool isExt = (chat.Type == (int) ChatType.Extension);
            if(!CheckError(code))
            {
                AddChatPerm(chat.ID, Role.Student, isExt);
            }
            return (chat.ID, DBCode.OK);
        }
        public (int, DBCode) InsertMessage(MessageInput input, bool isTeacher) {
            var msg = new Message {
                ChatID = input.ChatID,
                Body = input.Text,
                AttachedFilePath = input.FilePath,
                IsTeacher = isTeacher,
                SenderID = UserID,
                Status = 0,
            };
            (Chat chat, DBCode code) = Convert(DataBaseManager.ReadChat(input.ChatID));
            if(CheckError(code))
            {
                return (-1, code);
            }
            else if(chat.IsClosed)
            {
                return (-1, DBCode.NotAllowed);
            }
            return Convert(DataBaseManager.AddMessage(msg));
        }
        public DBCode MarkSubmitted(string submissionid, int styleGrade)
        {
            var res = Convert(DataBaseManager.ReadSubmission(submissionid));
            if(CheckError(res.Item2))
            {
                return res.Item2;
            }
            var sub = res.Item1;
            sub.SubmissionStatus = 1;
            sub.TimeSubmitted = DateTime.Now;
            sub.StyleGrade = styleGrade;
            return Convert(DataBaseManager.UpdateSubmission(sub));
        }
        public (string, DBCode) AddTeacherToCourse(string courseid, string teacherid)
        {
            return DataBaseManager.AddUserToCourse(courseid, teacherid, Role.Teacher);
        }
        public (string, DBCode) AddCheckerToCourse(string courseid, string checkerid)
        {
            return DataBaseManager.AddUserToCourse(courseid, checkerid, Role.Checker);
        }
         public (string, DBCode) AddStudentToCourse(string courseid, string studentId)
        {
            return DataBaseManager.AddUserToCourse(courseid, studentId, Role.Student);
        }
        public DBCode DeleteCheckerFromCourse(string courseid, string checkerid)
        {
            return DataBaseManager.DeleteUserFromCourse(courseid, checkerid, Role.Checker);
        }
        public DBCode AddCheckerToExercise(string courseid, string checkerid)
        {
            return DataBaseManager.AddCheckerToExercise(courseid, checkerid);
        }
        public DBCode DeleteCheckerFromExercise(string courseid, string checkerid)
        {
            return DataBaseManager.DeleteCheckerFromExercise(courseid, checkerid);
        }
        public (List<UserLabel>, DBCode) GetExerciseCheckers(string exerciseid)
        {
            return Convert(DataBaseManager.GetExerciseCheckers(exerciseid));
        }
        public (List<Test>, DBCode) GetTests(string exerciseid)
        {
            return Convert(DataBaseManager.ReadTestsOfExercise(exerciseid));
        }
        public (int, DBCode) AddTest(Test test)
        {
            return Convert(DataBaseManager.AddTest(test));
        }
        public DBCode CloseChat(string chatid)
        {
            return DataBaseManager.CloseChat(chatid);
        }
        public DBCode AcceptAppeal(string chatid)
        {
            (var chat, DBCode code2) = Convert(DataBaseManager.ReadChat(chatid));
            if(CheckError(code2))
            {
                return code2;
            }
            if(chat.Type == ChatType.Appeal)
            {
                return DBCode.Invalid;
            }
            (var sub, DBCode code3) = Convert(DataBaseManager.ReadSubmission(chat.SubmissionID));
            sub.SubmissionStatus = (int) SubmissionState.Appeal;
            sub.HasCopied = false;
            DBCode code4 = Convert(DataBaseManager.UpdateSubmission(sub));
            if(CheckError(code2))
            {
                return code2;
            }
            return CloseChat(chatid);
        }
        public DBCode AcceptExtension(string chatid, SubmitDate date)
        {
            (var chat, DBCode code) = Convert(DataBaseManager.ReadChat(chatid));
            if(CheckError(code))
            {
                return code;
            }
            (Submission submission, DBCode code2) = Convert(DataBaseManager.ReadSubmission(chat.SubmissionID));
            if(CheckError(code2))
            {
                return code2;
            }
            date.Group = -1;
            date.ExerciseID = submission.ExerciseID;
            (int dateid, DBCode code3) = DataBaseManager.AddDate(date);
            if(CheckError(code3))
            {
                return code3;
            }
            submission.SubmissionDateId = dateid;
            DBCode code4 = Convert(DataBaseManager.UpdateSubmission(submission));
            if(CheckError(code4))
            {
                return code4;
            }
            return CloseChat(chatid);
        }
        public (List<ExTeacherDisplay>, DBCode) GetTeacherExercises()
        {
            return Convert(DataBaseManager.ReadTeacherExerciseLabels(UserID));
        }
        public void AddMossCheck(MossData data)
        {
            return;
        }
        public DBCode DeletePasswordToken(string userID)
        {
            return Convert(DataBaseManager.DeleteToken(userID));
        }
        public DBCode DeleteTest(int testId)
        {
            return DataBaseManager.DeleteTest(testId);
        }
        public DBCode UpdateTest(Test test)
        {
            return Convert(DataBaseManager.UpdateTest(test));
        }
        public (string, DBCode) CheckPasswordToken(string token)
        {
            return DataBaseManager.CheckToken(token);
        }
        public DBCode AddPasswordToken(string userID, string token, DateTime expiration)
        {
            return DataBaseManager.AddPasswordToken(userID, token, expiration);
        }
        public (List<RequestLabel>, DBCode) GetRequests(string exerciseID, ChatType type)
        {
            return DataBaseManager.GetRequests(exerciseID, type);
        }
          public (List<RequestLabelMainPage>, DBCode) GetRequests(ChatType type)
        {
            return DataBaseManager.GetRequestsMainPage(UserID, type);
        }
        public (Exercise, DBCode) GetExercise(string exerciseID)
        {
            return Convert(DataBaseManager.ReadExercise(exerciseID));
        }
         public (int, DBCode) AddDate(SubmitDate date)
        {
            return DataBaseManager.AddDate(date);
        }
        public DBCode UpdateDate(SubmitDate date)
        {
            return DataBaseManager.UpDate(date);
        }
        public DBCode DeleteDate(int dateId)
        {
            return DataBaseManager.DeleteDate(dateId);
        }
        public (List<TeacherDateDisplay>,DBCode) GetExerciseDates(string exerciseID)
        {
            return DataBaseManager.GetDatseOfExercise(exerciseID);
        }
        public DBCode MarkCopy(CopyForm copy)
        {
            return DataBaseManager.SetCopied(copy);
        }
        public (Dictionary<string, List<SubmissionLabel>>, DBCode) GetSubmissionLabels(string exerciseID)
        {
            (var dict, DBCode code) = Convert(DataBaseManager.GetSubmissionLabels(exerciseID));
            string appealCheckedStr = SubmissionState.AppealChecked.ToString();
            var appealChecked = dict[appealCheckedStr];
            dict.Remove(appealCheckedStr);
            dict[SubmissionState.Checked.ToString()].AddRange(appealChecked);
            dict.Remove(SubmissionState.Unsubmitted.ToString());
            foreach(var list in dict.Values)
            {
                list.ForEach(label => label.CheckState = GetCheckState(label.CurrentChecker));
                list.Sort(delegate(SubmissionLabel x, SubmissionLabel y)
                {
                    return x.CheckState.CompareTo(y.CheckState);
                });
            }
            return (dict, code);
        }
        /// <summary>
        /// Gets a submission and marks the user as it's current checker. Only for teachers and checkers.
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        public (Submission, DBCode) GetSubmission(string submissionId)
        {
            (Submission submission, DBCode code) = Convert(DataBaseManager.ReadSubmission(submissionId));
            if(CheckError(code))
            {
                return (submission, code);
            }
            submission.CheckState = GetCheckState(submission.CurrentCheckerId);
            (submission.TotalGrade, code) = DataBaseManager.CalculateGrade(submissionId);
            return (submission, code);
           
        }
        public DBCode BeginChecking(string submisisonId)
        {
            return DataBaseManager.BeginChecking(submisisonId, UserID);
        }
        private CheckState GetCheckState(string checkerId)
        {
            if(String.IsNullOrEmpty(checkerId))
            {
                return CheckState.NotBeingChecked;
            }
            else if(checkerId == UserID)
            {
                return CheckState.CheckedByYou;
            }
            else
            {
                return CheckState.CheckedByOther;
            }
        }
        public (string, DBCode) GetMessageFile(int messageId)
        {
            return DataBaseManager.GetMessageFile(messageId);
        }
         public (string, DBCode) GetTestDir(int testId)
        {
            return DataBaseManager.GetTestDir(testId);
        }
        public DBCode EndChecking(Submission submission)
        {
            return DataBaseManager.EndChecking(submission);
        }
        public DBCode AddCourse(Course course)
        {
            return Convert(DataBaseManager.AddCourse(course));
        }
        public DBCode DetachStudent(string submissionId)
        {
            (int type, DBCode code) = Convert(DataBaseManager.ReadTypeOfStudentInSubmission(submissionId, UserID));
            if(code != DBCode.OK)
            {
                return code;
            }
            if(type == 1)
            {
                return DBCode.NotAllowed;
            }
            return Convert(DataBaseManager.DeleteStudentFromSubmission(UserID, submissionId));
        }
        public (Dictionary<string, bool>, DBCode) ValidateSubmitters(string exerciseId, List<string> submitters)
        {
            submitters.Remove(UserID);
            var subDict = new Dictionary<string, string>();
            var dict = new Dictionary<string ,bool>();
            ((string submissionId, int type), DBCode code) = Convert(DataBaseManager.ReadSubmissionIdAndTypeOfStudentInExercise(UserID, exerciseId));
            if(code != DBCode.OK)
            {
                return (null, code);
            }
            if(type != 1)
            {
                return (null, DBCode.NotAllowed);
            }
            (List<UserLabel> currentSubmitters, DBCode codeb) = DataBaseManager.GetSubmitters(submissionId);
            if(code != DBCode.OK)
            {
                return (null, code);
            }
            var ids = currentSubmitters.Select((x)=>x.ID);
            submitters = submitters.Except(ids).ToList();
            (Exercise exercise, DBCode codea) = GetExercise(exerciseId);
            if(codea != DBCode.OK)
            {
                return (null, codea);
            }
            if(exercise.MaxSubmitters < submitters.Count + currentSubmitters.Count)
            {
                return (null, DBCode.NotAllowed);
            }
           foreach(string submitter in submitters)
           {
                ((string subId, _), DBCode code2) = Convert(DataBaseManager.ReadSubmissionIdAndTypeOfStudentInExercise(submitter, exerciseId));
                if(code2 == DBCode.NotFound)
                {
                    var c = DataBaseManager.CheckResourcePerms(exerciseId, submitter, Role.Student, ResourceType.Exercise);
                    if(c != DBCode.OK)
                    {
                        dict[submitter] = false;
                    }
                    else
                    {   
                        dict[submitter] = true;
                    }
                    continue;
                }
                else if(code2 == DBCode.Error)
                {
                    return (null, DBCode.Error);
                }
                subDict[submitter]  = subId;
                if(subId == submissionId)
                {
                    dict[submitter] = true;
                }
                (Submission sub, DBCode code3) = Convert(DataBaseManager.ReadSubmission(subId));
                if(code3 == DBCode.Error)
                {
                    return (null, DBCode.Error);
                }
                else if(code3 == DBCode.NotFound)
                {
                    DataBaseManager.DeleteStudentFromSubmission(submitter, subId);
                    dict[submitter] = true;
                }
                else
                {
                    dict[submitter] = ((SubmissionState) sub.SubmissionStatus == SubmissionState.Unsubmitted);
                }
           }
           if(dict.Values.ToList().All((entry) => entry))
           {
                foreach(var studentAndSubmission in subDict)
                {
                    string subId = studentAndSubmission.Value;
                    if(subId == submissionId)
                    {
                        continue;
                    }
                    string student = studentAndSubmission.Key;
                    var a = DataBaseManager.DeleteSubmission(subId);
                    if(a != DBCode.OK)
                    {
                        return (null, a);
                    }
                    a = Convert(DataBaseManager.DeleteStudentFromSubmission(student, studentAndSubmission.Value));
                    if(a != DBCode.OK)
                    {
                        return (null, a);
                    }
                }
                foreach(string submitter in dict.Keys)
                {
                    if(subDict.ContainsKey(submitter) && subDict[submitter] == submissionId)
                    {
                        continue;
                    }
                    code = Convert(DataBaseManager.AddStudentToSubmission(submissionId, submitter, 0, exerciseId));
                    if(code != DBCode.OK)
                    {
                        return (null, DBCode.Error);
                    }
                }
           }
            return (dict, DBCode.OK);
        }
        public (List<User>, DBCode) GetSubmitters(string submissionId)
        {
            return DataBaseManager.GetSubmittersWithEmail(submissionId);
        }
        public DBCode SetPassword(string userid, string hash)
        {
            return DataBaseManager.UpdatePassword(userid, hash);
        }

        public DBCode UpdateExercise(Exercise exercise)
        {
            return Convert(DataBaseManager.UpdateExercise(exercise));
        }
        public (User, DBCode) GetUser(string userID)
        {
            return Convert(DataBaseManager.ReadUser(userID));
        }
        public DBCode AddUser(User user)
        {
            return Convert(DataBaseManager.AddUser(user));
        }
        public DBCode CancelSubmission(string submissionId)
        {
            (var submission, DBCode code) = Convert(DataBaseManager.ReadSubmission(submissionId));
            (int type, DBCode code2) = Convert(DataBaseManager.ReadTypeOfStudentInSubmission(submissionId, UserID));
            if(code != DBCode.OK || code != DBCode.OK)
            {
                return code2;
            }
            else if(type == 0)
            {
                return DBCode.NotAllowed;
            }
            (Exercise exercise, DBCode code3) = Convert(DataBaseManager.ReadExercise(submission.ExerciseID));
            if(code3 != DBCode.OK)
            {
                return code3;
            }
            if(!exercise.MultipleSubmission)
            {
                return DBCode.NotAllowed;
            }
            submission.SubmissionStatus = (int) SubmissionState.Unsubmitted;
            return Convert(DataBaseManager.UpdateSubmission(submission));
        }
        public DBCode DeleteExercise(string exerciseId)
        {
            return DataBaseManager.DeleteExercise(exerciseId);
        }
    }
}