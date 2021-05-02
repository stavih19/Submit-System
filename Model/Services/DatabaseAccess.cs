using System;
using System.Collections.Generic;
namespace Submit_System {
    public enum CourseRole { Student, Teacher }
    public class DatabaseAccess {

        private readonly string[] _tableMapper = new string[2] {"Student", "Teacher"};
        private const string ALL_SUBMITTERS = "";
        public List<Course> GetCourses(string UserId, CourseRole role) {
            return FakeDatabase.CourseList;
        }
        public List<ExerciseLabel> GetCourseExercises(string courseId) {
            var fullExes = FakeDatabase.ExcerciseList;
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
            var exes = FakeDatabase.ExcerciseList;
            foreach(var ex in exes)
            {
                var newGrade = new ExerciseGradeDisplay {
                    Grade = 100,
                    ExID = ex.ID,
                    ExName = ex.Name,
                    CourseName = "תכנות מתקדם 1",
                    CourseNumber = 89111
                };
                grades.Add(newGrade);
            };
            return grades;
        }

        public List<ExerciseDateDisplay> GetExcercisesDates(string userId) {
            var fullEx = FakeDatabase.ExcerciseList[1];
            var ex = new ExerciseDateDisplay {
                ID = fullEx.ID,
                Name = fullEx.Name,
                Date = fullEx.Dates[0].date,
                CourseName = "תכנות מתקדם 1",
                CourseNumer = 89111
                
            };
            return new List<ExerciseDateDisplay> {ex};
        }
        public void SubmitExercise(FullSubmission data) {}
    }
}