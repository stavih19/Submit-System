using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Diagnostics;
namespace Submit_System {
    public class FakeDatabaseAccess : IDatabaseAccess {

        public string GetDirectory(string userid, string submisisonId)
        {
            foreach(var a in FakeDatabase.Submissions)
            {
                if(a.ID == submisisonId && Directory.Exists(a.Folder))
                {
                    return a.Folder;
                }
            }
            return null;
        }
        public string CreateDirectoryPath(string userid, string exerciseId)
        {
           foreach(var a in FakeDatabase.Submissions)
            {
                if(a.ExID == exerciseId)
                {
                    return a.Folder;
                }
            }
            return null;
        }
        public List<Course> GetCourses(string UserId, Role role) {
            return FakeDatabase.CourseList;
        }
        public List<ExerciseLabel> GetCourseExercises(string courseId) {
            var fullExes = FakeDatabase.ExerciseList;
            var a = new List<ExerciseLabel>();
            if(courseId != FakeDatabase.C2ID && courseId != FakeDatabase.C1ID)
            {
                return null;
            }
            foreach(var ex in fullExes)
            {
                if(ex.CourseID == courseId)
                {
                    var newEx = new ExerciseLabel {
                        ID = ex.ID,
                        Name = ex.Name
                    };
                     a.Add(newEx);

                } 
            }
            return a;
        }

        public List<CheckerExInfo> GetExercises(string userid)
        {
            var exes = FakeDatabase.ExerciseList;
            var ExInfos = new List<CheckerExInfo>();
            foreach(var ex in exes)
            {
                var newInfo = new CheckerExInfo {
                    
                    ExID = ex.ID,
                    ExName = ex.Name,
                    CourseID = "89111-2021",
                    CourseName = "תכנות מתקדם 1",
                    CourseNumber = 89111,
                    NumOfSubmissions = 1,
                    NumOfAppeals = 10
                };
                ExInfos.Add(newInfo);
            };
            return ExInfos;
        }
        public List<SubmissionData> GetSubmissions(string exId=null, string studentId=null)
        {
            if(exId == null && studentId == null)
            {
                throw new ArgumentNullException("Both the student ID and Exercise ID can't be null.");
            }
            return FakeDatabase.Submissions;
        }   
        public List<ExerciseGradeDisplay> GetStudentGrades(string id) {
            var grades = new List<ExerciseGradeDisplay>();
            var exes = FakeDatabase.ExerciseList;
            foreach(var ex in exes)
            {
                var newGrade = new ExerciseGradeDisplay {
                    Grade = 100,
                    ExID = ex.ID,
                    ExName = ex.Name,
                    CourseID = "89111-2021",
                    CourseName = "תכנות מתקדם 1",
                    CourseNumber = 89111
                };
                grades.Add(newGrade);
            };
            return grades;
        }

        public List<ExerciseDateDisplay> GetExcercisesDates(string userId) {
            var fullEx = FakeDatabase.ExerciseList[1];
            var ex = new ExerciseDateDisplay {
                ID = fullEx.ID,
                Name = fullEx.Name,
                Date = fullEx.Dates[0].date,
                TeacherName = "Avi Avi",
                CourseID = "89111-2021",
                CourseName = "תכנות מתקדם 1",
                CourseNumer = 89111
                
            };
            return new List<ExerciseDateDisplay> {ex};
        }
        public StudentExInfo GetStudentSubmission(string studentId, string exId)
        {
            var subs = FakeDatabase.Submissions;
            var exs = FakeDatabase.ExerciseList;
            StudentExInfo info = new StudentExInfo();
            bool found = false;
            foreach(var ex in exs)
            {
                if(exId == ex.ID)
                {
                    info.ExID = ex.ID;
                    info.ExName = ex.Name;
                    info.MaxSubmitters = ex.MaxSubmitters;
                    var date = ex.Dates[0];
                    info.Date = date.date;
                    info.MaxLateDays = ex.MaxLateDays;
                    info.LateDayPenalty = ex.LateDayPenalty;
                    info.IsMultipleSubmission = ex.IsMultipleSubmission;
                    found = true;
                }
            }
             foreach(var sub in subs)
            {
                if(exId == sub.ExID)
                {
                    info.ManualGrade = sub.ManualGrade;
                    info.SubmissionID = sub.ID;
                    info.AutoGrade = sub.AutoGrade;
                    info.StyleGrade = sub.StyleGrade;
                    info.AutoGrade = sub.AutoGrade;
                    info.TotalGrade = sub.TotalGrade;
                    info.AppealChat = sub.AppealChat;
                    info.ExtensionChat = sub.ExtensionChat;
                    info.filenames = new List<string>();
                    info.filenames = FileUtils.GetRelativePaths(sub.Folder);
                    info.State = sub.State;
                    info.Submitters = sub.Submitters;
                }
            }
            if(!found)
            {
                return null;
            }
            return info;
        }
        public List<Message> GetMesssages(string userId, string chatId)
        {
            return new List<Message> { FakeDatabase.Msg, FakeDatabase.Msg2 };
        }
        public void InsertMessage(string userId, string chatId, string text) {
            var msg = new Message {
                Body = text,
                SenderID = userId,
                ChatID = chatId,
                ID = "b",
                Date = DateTime.Now,
                IsTeacher = false
            };
        }
        public string GetSubmissionPath(string userId, string submissionId) {
            return "path/";
        }
        public string GetExercisePath(string userId, string exerciseId) {
            return "path/";
        }

    }
}