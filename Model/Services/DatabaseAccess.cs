using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
namespace Submit_System {
    public enum DBCode { OK, NotFound, Invalid, Error, NotAllowed, AlreadyExists }
    public enum Role { Student, Checker, Teacher }
    public class DatabaseAccess {
        private readonly IMemoryCache _permissions;
        public DatabaseAccess()
        {
            _permissions = new MemoryCache( new MemoryCacheOptions { SizeLimit = 100 });
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
            }
            return (DBCode) output.code;
        }
        private UserPermissions GetPermission(string userid)
        {
            var perms = _permissions.GetOrCreate(userid, entry => {
                entry.SlidingExpiration = TimeSpan.FromMinutes(75);
                entry.SetSize(1);
                return new UserPermissions();
            });
            return perms;
        }
        public DBCode CheckCoursePermission(string userid, string courseid, Role role)
        {
            var perms = GetPermission(userid);
            bool has = false;
            Monitor.Enter(perms);
            has = perms.CheckCoursePerm(courseid, role);
            Monitor.Exit(perms);
            if(has) return DBCode.OK;
            // TODO do a real prmission check
            if(role == Role.Teacher) return DBCode.OK;
            (bool inCourse, DBCode code) = Convert(DataBaseManager.IsStudentInCourse(courseid, userid));
            if(inCourse)
            {
                AddCoursePerms(courseid, userid, Role.Student);
            }
            return code;
        }
        public void AddCoursePerms(string courseid, string userid, Role role)
        {
            AddCoursePerms(new List<string>{courseid}, userid, Role.Student);
        }
        public void AddCoursePerms(List<string> courseids, string userid, Role role)
        {
            var perms = GetPermission(userid);
            Monitor.Enter(perms);
            foreach(var id in courseids)
            {
                perms.SetCoursePerm(id, role);
            }
            Monitor.Exit(perms);
        }
        public void AddExercisePerm(string exerciseid, string userid, Role role)
        {
            var perms = GetPermission(userid);
            Monitor.Enter(perms);
            perms.SetExercisePerm(exerciseid, role);
            Monitor.Exit(perms);
        }
        public void AddExerciseTeacherPerm(string exerciseid, string userid, string exFolder)
        {
            var perms = GetPermission(userid);
            Monitor.Enter(perms);
            perms.SetTeacherExercisePerm(exerciseid, exFolder);
            Monitor.Exit(perms);
        }
        public (string, DBCode) GetExerciseDirectory(string exerciseID, string userID)
        {
            UserPermissions perms = GetPermission(userID);
            string folder = perms.GetExerciseFolder(exerciseID);
            if(folder == null)
            {
                DBCode code = DBCode.OK; //CheckExercisePerm(exerciseID, userID, Role.Teacher);
                if(CheckError(code))
                {
                    return (null, code);
                }
                (Exercise ex, DBCode code2) = Convert(DataBaseManager.ReadExercise(exerciseID));
                if(CheckError(code2))
                {
                    return (null, code);
                }
                AddExerciseTeacherPerm(exerciseID, userID, ex.FilesLocation);
                return (ex.FilesLocation, DBCode.OK);
            }
            return (folder, DBCode.OK);

        }
        public DBCode CheckExercisePerm(string exerciseid, string userid, Role role)
        {
            var perms = GetPermission(userid);
            bool has;
            Monitor.Enter(perms);
            has = perms.CheckExercisePerm(exerciseid, role);
            Monitor.Exit(perms);
            if(has)
            {
                return DBCode.OK;
            }
            return CheckResourcePerm(exerciseid, userid, role, ResourceType.Exercise);
        }
        public void AddSubmissionPerm(string submissionid, string userid, Role role, string folder)
        {
            var perms = GetPermission(userid);
            Monitor.Enter(perms);
            perms.SetSubmissionPerm(submissionid, role, folder);
            Monitor.Exit(perms);
        }
        public DBCode CheckSubmissionPerm(string submissionid, string userid, Role role)
        {
            var perms = GetPermission(userid);
            bool has;
            Monitor.Enter(perms);
            has = perms.CheckSubmissionPerm(submissionid, role);
            Monitor.Exit(perms);
            if(has)
            {
                return DBCode.OK;
            }
            return CheckResourcePerm(submissionid, userid, role, ResourceType.Submission);
        }
        public DBCode CheckChatPerm(string chatid, string userid, Role role)
        {
            var perms = GetPermission(userid);
            bool has;
            Monitor.Enter(perms);
            has = perms.CheckSubmissionPerm(chatid, role);
            Monitor.Exit(perms);
            if(has)
            {
                return DBCode.OK;
            }
            return CheckResourcePerm(chatid, userid, role, ResourceType.Chat);
        }
        public void AddChatPerm(string chatid, string userid, Role role, bool isExt)
        {
            var perms = GetPermission(userid);
            Monitor.Enter(perms);
            perms.SetChatPerm(chatid, role, isExt);
            Monitor.Exit(perms);
        }
        public bool CheckError(DBCode code)
        {
            return CheckError((int) code);
        }
        public bool CheckError(int code)
        {
            return code != 0 && code != 4;
        }
        public (string, DBCode) AuthenticateUser(string id, string password)
        {
            (User user, DBCode code) = Convert(DataBaseManager.ReadUser(id));
            if(CheckError(code))
            {
                return (null, code);
            }
            if(CryptoUtils.Verify(password, user.PasswordHash))
            {
                return (user.Name, code);
            }
            return (null, DBCode.NotFound);
        }
        public (string, DBCode) GetSubmissionDirectory(string userid, string submisisonId, Role role)
        {
            var perm = GetPermission(userid);
            var folder = perm.GetSubmissionFolder(submisisonId);
            if(folder == null)
            {
                (int type, DBCode code) = Convert(DataBaseManager.ReadTypeOfStudentInSubmission(submisisonId, userid));
                if(CheckError(code))
                {
                    return (null, code);
                }
                (Submission submission, DBCode code2) = Convert(DataBaseManager.ReadSubmission(submisisonId));
                folder = submission?.Files_Location;
                AddSubmissionPerm(submisisonId, userid, role, folder);
                return (folder,code2);
            }
            return (folder, DBCode.OK);
        }
        public (string, string, DBCode) CreateSubmissionPath(string userid, string exerciseId)
        {
           var ex = Convert(DataBaseManager.ReadExercise(exerciseId));
           var subo = Convert(DataBaseManager.ReadSubmissionIdAndTypeOfStudentInExercise(userid, exerciseId));
           if(subo.Item1.Item2 != 1)
           {
               return (null, null, DBCode.NotAllowed);
           }
           var subosis = Convert(DataBaseManager.ReadSubmission(subo.Item1.Item1));
           if(subosis.Item1.Submission_Status != 0 && ex.Item1.MultipleSubmission)
           {
               return (null, null, DBCode.NotAllowed);
           }
           if(CheckError(ex.Item2))
           {
               return (null, null, (DBCode) ex.Item2);
           }
           Exercise exer = ex.Item1;
           var code = CheckCoursePermission(userid, exer.Course_ID, Role.Student);
           if(code != DBCode.OK)
           {
               return (null, null, code);
           }
           string path = Path.Combine("Courses", exer.Course_ID, "Exercises", exer.Name, "Submissions", userid);
           return (path, subosis.Item1.ID, DBCode.OK);

        }
        public (List<Course>, DBCode) GetCourses(string userid, Role role) {
            return Convert(DataBaseManager.GetCoursesOfUser(userid, role));
        }
        public (List<ExerciseLabel>, DBCode) GetCourseExercises(string courseId, string userid, Role role) {
            
            var result = CheckCoursePermission(userid, courseId, role);
            if(CheckError(result))
            {
                return (null, result);
            }
            var exes = Convert(DataBaseManager.ReadExerciseLabelsOfCourse(courseId));
            return exes;
        }

        public List<CheckerExInfo> GetExercises(string userid)
        {
          return null;
        }
        public (List<ExerciseGradeDisplay>, DBCode) GetStudentGrades(string userid)
        {
            return Convert(DataBaseManager.GetExerciseGrades(userid));
        }

        public (SortedDictionary<string, List<ExerciseLabel>>, DBCode) GetAllExercises(string userid, string courseid)
        {
            var res = CheckCoursePermission(userid, courseid, Role.Teacher);
            if(CheckError(res))
            {
                return (null, res);
            }
            return Convert(DataBaseManager.GetAllExercises(courseid));
        }

        public (List<UserLabel>, DBCode) GetCourseTeachers(string userid, string courseid)
        {
            return Convert(DataBaseManager.ReadUsersOfCourse(courseid, Role.Teacher));
        }
        public (List<UserLabel>, DBCode) GetCourseCheckers(string userid, string courseid)
        {
            return Convert(DataBaseManager.ReadUsersOfCourse(courseid, Role.Checker));
        }
        public Submission CreateSubmission(string studentId, string exId)
        {
            var s = new Submission {
                ID = Submission.GenerateID(studentId, exId),
                Exercise_ID = exId,
                Submission_Status = (int) SubmissionState.Unchecked,
                Auto_Grade = -1,
                Style_Grade = -1,
                Manual_Final_Grade = -1,
                Time_Submitted = DateTime.MinValue,
            };
            DataBaseManager.AddSubmission(s);
            DataBaseManager.AddStudentToSubmission(s.ID, studentId, 1, exId);
            return s;
        }
        private DBCode CheckResourcePerm(string dateid, string userid, Role role, ResourceType resource)
        {
            var a = Convert(DataBaseManager.CheckResourcePerms(dateid, userid, role, resource));
            if(a.Item1)
            {
                return DBCode.OK;
            }
            else if(a.Item2 == DBCode.OK)
            {
                return DBCode.NotFound;
            }
            return a.Item2;
        }
        public DBCode CheckDatePerm(string userid, string dateid)
        {
            return CheckResourcePerm(dateid, userid, Role.Teacher, ResourceType.Date);
        }
        public (List<ExerciseDateDisplay>, DBCode) GetExcercisesDates(string userId) {
           var output = Convert(DataBaseManager.GetExerciseDates(userId));
           return output;
        }
        public (StudentExInfo, DBCode) GetStudentSubmission(string userid, string exId)
        {
            var exResult = Convert(DataBaseManager.ReadExercise(exId));
            if(CheckError(exResult.Item2))
            {
                return (null, exResult.Item2);
            }
            var ex = exResult.Item1;
            AddExercisePerm(ex.ID, userid, Role.Student);
            ((string id, int type), DBCode code) = Convert(DataBaseManager.ReadSubmissionIdAndTypeOfStudentInExercise(userid, exId));
            Submission submission;
            if(code == DBCode.NotFound) {
                DBCode hasPerm = CheckCoursePermission(userid, ex.Course_ID, Role.Student);
                submission = CreateSubmission(userid, exId);
            }
            if(CheckError(code))
            {
                 return (null, code);
            }
            (var submissionTemp, DBCode code2) = Convert(DataBaseManager.ReadSubmission(id));
            submission = submissionTemp;
            if(CheckError(code2))
            { 
                return (null, code2);
            }
            AddSubmissionPerm(id, userid, Role.Student, submission.Files_Location);
            (var chats, DBCode code3) = Convert(DataBaseManager.ReadChatsOfSubmission(id));
            if(CheckError(code3))
            { 
                return (null, code3);
            }
            if(chats[ChatType.Extension] != null)
            {
                AddChatPerm(chats[ChatType.Extension].ID, userid, Role.Student, true);
            }
             if(chats[ChatType.Appeal] != null)
            {
                AddChatPerm(chats[ChatType.Appeal].ID, userid, Role.Student, false);
            }
            (var submitters, DBCode code4) = Convert(DataBaseManager.GetSubmitters(id));
            if(CheckError(code4))
            { 
                return (null, code4);
            }
            (var dates, DBCode code5) = Convert(DataBaseManager.GetStudentExeriseDates(submission.Submission_Date_Id, exId));
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
                DateSubmitted = submission.Time_Submitted,
                AutoWeight = ex.AutoTestGradeWeight,
                StyleWeight = ex.StyleTestGradeWeight,
                AutoGrade = submission.Auto_Grade,
                StyleGrade = submission.Style_Grade,
                ManualGrade = submission.Manual_Final_Grade,
                SubmissionFolder = submission.Files_Location,
                ExtensionChat = chats[ChatType.Extension],
                LateSubmissionSettings = ex.LateSubmissionSettings,
                AppealChat = chats[ChatType.Appeal],
                MaxSubmitters = ex.MaxSubmitters,
                Dates = dates,
                IsMultipleSubmission = ex.MultipleSubmission,
                State = (SubmissionState) submission.Submission_Status,
                InitReduction = r,
                Submitters = submitters,
                IsMainSubmitter = (type == 1)
            };
            return (info, DBCode.OK);
        }
        public (List<Message>, DBCode) GetMesssages(string userId, string chatId, Role role)
        {
            DBCode res = CheckChatPerm(chatId, userId, role);
            if(res != DBCode.OK)
            {
                return (null, res);
            }
            return Convert(DataBaseManager.ReadMessagesOfChat(chatId));
        }
        public DBCode Appeal(string userId, string submissionid, MessageInput firstMessage)
        {
            var code = CheckSubmissionPerm(submissionid, userId, Role.Student);
            if(CheckError(code))
            {
                return code;
            }
            var chat = new ChatData(submissionid, (int) ChatType.Appeal);
            return AddChat(userId, chat, firstMessage);
        }
        public DBCode Extension(string userId, string submissionid, MessageInput firstMessage)
        {
            var chat = new ChatData(submissionid, (int) ChatType.Extension);
            return AddChat(userId, chat, firstMessage);
        }
        private DBCode AddChat(string userId, ChatData chat, MessageInput firstMassage)
        {
            DBCode code = CheckSubmissionPerm(chat.Submission_ID, userId, Role.Student);
            if(CheckError(code))
            {
                return code;
            }
            code = Convert(DataBaseManager.AddChat(chat));
            bool a = chat.Type == (int) ChatType.Extension;
            AddChatPerm(chat.ID, userId, Role.Student, a);
            if(CheckError(code))
            {
                return code;
            }
            return InsertStudentMessage(userId, chat.ID, firstMassage.Text);
        }
        public DBCode InsertStudentAttachment(string userId, string chatId, SubmitFile file) {
            DBCode res = CheckChatPerm(chatId, userId, Role.Student);
            if(res != DBCode.OK)
            {
                return res;
            }
            string hasho =  CryptoUtils.GetRandomBase64String(6).Replace('/', '-');
            string path = Path.Combine("Requests", hasho);
            while(Directory.Exists(path)) {
                hasho =  CryptoUtils.GetRandomBase64String(6);
            }
            file.CreateFile(path);
            var msg = new MessageData(chatId, DateTime.Now, 1, path, userId, 0, "userid");
            return Convert(DataBaseManager.AddMessage(msg));
        }
        public DBCode InsertStudentMessage(string userId, string chatId, string text) {
            DBCode res = CheckChatPerm(chatId, userId, Role.Student);
            if(res != DBCode.OK)
            {
                return res;
            }
            var msg = new MessageData(chatId, DateTime.Now, 0, text, userId, 0, "userid");
            return Convert(DataBaseManager.AddMessage(msg));
        }
        public DBCode InsertTeacherMessage(string userId, string chatId, MessageInput msg) {
            DBCode res = CheckChatPerm(chatId, userId, Role.Student);
            if(res != DBCode.OK)
            {
                return res;
            }
            var msga = new MessageData();
            return Convert(DataBaseManager.AddMessage(msga));
        }
        public DBCode MarkSubmitted(string submissionid, string path)
        {
            var res = Convert(DataBaseManager.ReadSubmission(submissionid));
            if(CheckError(res.Item2))
            {
                return res.Item2;
            }
            var sub = res.Item1;
            sub.Submission_Status = 1;
            sub.Files_Location = path;
            return Convert(DataBaseManager.UpdateSubmission(sub));
        }
        public DBCode AddTeacherToCourse(string userid, string courseid, string teacherid)
        {
            DBCode code = CheckCoursePermission(userid, courseid, Role.Teacher);
            if(code == DBCode.OK)
            {
                code = Convert(DataBaseManager.AddUserToCourse(courseid, teacherid, Role.Teacher));
            }
            return code;
        }
        public DBCode AddCheckerToCourse(string userid, string courseid, string checkerid)
        {
            DBCode code = CheckCoursePermission(userid, courseid, Role.Teacher);
            if(code == DBCode.OK)
            {
                code = Convert(DataBaseManager.AddUserToCourse(courseid, checkerid, Role.Checker));
            }
            return code;
        }
        public DBCode DeleteCheckerFromCourse(string userid, string courseid, string checkerid)
        {
            DBCode code = CheckCoursePermission(userid, courseid, Role.Teacher);
            if(code == DBCode.OK)
            {
                code = Convert(DataBaseManager.DeleteUserFromCourse(courseid, checkerid, Role.Checker));
            }
            return code;
        }
        public DBCode AddCheckerToExercise(string userid, string courseid, string checkerid)
        {
            DBCode code = CheckCoursePermission(userid, courseid, Role.Teacher);
            if(code == DBCode.OK)
            {
                code = Convert(DataBaseManager.AddCheckerToExercise(courseid, checkerid));
            }
            return code;
        }
        public DBCode DeleteCheckerFromExercise(string userid, string courseid, string checkerid)
        {
            DBCode code = CheckCoursePermission(userid, courseid, Role.Teacher);
            if(code == DBCode.OK)
            {
                code = Convert(DataBaseManager.DeleteCheckerFromExercise(courseid, checkerid));
            }
            return code;
        }
        public (List<UserLabel>, DBCode) GetExerciseCheckers(string userid, string exerciseid)
        {
            return Convert(DataBaseManager.GetExerciseCheckers(exerciseid));
        }
        public (List<Test>, DBCode) GetTests(string userid, string exerciseid)
        {
            return Convert(DataBaseManager.ReadTestsOfExercise(exerciseid));
        }
        public DBCode AddTest(string userid, Test test)
        {
            if(CheckExercisePerm(test.Exercise_ID, userid, Role.Teacher) != DBCode.OK)
            {
                return DBCode.NotAllowed;
            }
            return Convert(DataBaseManager.AddTest(test));
        }
        public DBCode CloseChat(string userid, string chatid)
        {
            DBCode code = CheckChatPerm(chatid, userid, Role.Teacher);
            if(CheckError(code))
            {
                return code;
            }
            return CloseChat(chatid);
        }
        private DBCode CloseChat(string chatid)
        {
            return Convert(DataBaseManager.CloseChat(chatid));
        }
        public (List<ExTeacherDisplay>, DBCode) GetTeacherExercises(string userid)
        {
            return Convert(DataBaseManager.ReadTeacherExerciseLabels(userid));
        }
        public void AddMossCheck(MossData data)
        {
            return;
        }
        public (MossData, DBCode) GetMossCheck(string userID, string exerciseID)
        {
            DBCode code = CheckExercisePerm(exerciseID, userID, Role.Teacher);
            if(CheckError(code))
            {
                return (null, code);
            }
            (MossData data, DBCode code2) = (null, DBCode.NotFound); // TODO: Get MossCheck from Database
            if(CheckError(code2))
            {
                return (null, code2);
            }
            return (data, code2);
        }
        public (string, DBCode) GetExFolder(string exerciseID, string userID)
        {
            (string folder, DBCode code) = GetExerciseDirectory(exerciseID, userID);
            if(CheckError(code))
            {
                return (null, code);
            }
            return (folder, DBCode.OK);
        }
        public void DeletePasswordToken(string userID)
        {
            DataBaseManager.DeleteToken(userID);
        }
        public bool CheckPasswordToken(string token)
        {
            return(CheckError(Convert(DataBaseManager.ReadToken(token)).Item2));
        }
        public ((string, DateTime), DBCode) GetPasswordToken(string userID)
        {
            return Convert(DataBaseManager.ReadToken(userID));
        }
        public void AddPasswordToken(string userID, string token)
        {
            DataBaseManager.AddPasswordToken(userID, token);
        }
        public (List<RequestLabel>, DBCode) GetRequests(string exerciseID, string userid, ChatType type)
        {
            return Convert(DataBaseManager.GetRequests(exerciseID, type));
        }
    }
}