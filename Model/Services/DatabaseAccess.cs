using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
namespace Submit_System {
    public enum CourseRole { Student, Teacher }
    public class DatabaseAccess {

        private readonly string[] _tableMapper = new string[2] {"Student", "Teacher"};
        private const string ALL_SUBMITTERS = "";
        public List<Course> GetCourses(string UserId, CourseRole role) {
            return FakeDatabase.CourseList;
        }
        public List<ExerciseLabel> GetCourseExercises(string courseId) {
            var fullExes = FakeDatabase.ExerciseList;
            var a = new List<ExerciseLabel>();
            foreach(var ex in fullExes)
            {
                var newEx = new ExerciseLabel {
                    ID = ex.ID,
                    Name = ex.Name,
                };
                a.Add(newEx);
            };
            return a;
        }
        public List<FullSubmission> GetSubmissions(string exId=null, string studentId=null)
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
                TeacherName = "Yosi Yosi",
                CourseID = "89111-2021",
                CourseName = "תכנות מתקדם 1",
                CourseNumer = 89111
                
            };
            return new List<ExerciseDateDisplay> {ex};
        }
        public StudentExInfo GetStudentSubmission(string studentId, string exId)
        {
            var sub = FakeDatabase.Submissions[0];
            var ex = FakeDatabase.ExerciseList[0];
            var x = new StudentExInfo {
                ExName = ex.Name,
                SubmissionID = sub.ID,
                TotalGrade = sub.TotalGrade,
                ManualGrade = sub.ManualGrade,
                StyleGrade = sub.StyleGrade,
                AutoGrade = sub.AutoGrade,
                Dates = ex.Dates,
                Submitters = sub.Submitters,
                AppealChat = sub.AppealChat,
                ExtensionChat = sub.ExtensionChat,
                IsMultipleSubmission = ex.IsMultipleSubmission,
                MaxSubmitters = ex.MaxSubmitters,
                ExID = ex.ID
            };
            return x;
        }
        public void SubmitExercise(string userId, string exId, SubmissionUpload data) {
            // string path = $"Courses/{CourseId}/Exercises/{exId}/Submissions/{newId}/";
            string path = "/";
            foreach (IFormFile file in data.files) {
            if (file.Length > 0) {
                string filePath = Path.Combine(path, file.FileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
                    file.CopyTo(fileStream);
                }
            }
        }

        }
        public List<Message> GetMesssages(string userId, string chatId)
        {
            var msg = FakeDatabase.Msg;
            return new List<Message> { msg };
        }
        public void InsertMessage(string userId, string chatId, string text) {
            var msg = new Message {
                Body = text,
                SenderID = userId,
                ChatID = chatId,
                ID = "b",
                Date = DateTime.Now
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