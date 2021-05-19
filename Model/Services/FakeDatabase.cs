using System.Collections.Generic;
using System;
namespace Submit_System {
    /// <summary>
    ///     Fake Database for testing purposes
    /// </summary>
    public static class FakeDatabase {
        public static readonly List<Course> CourseList = new List<Course>
        {
            new Course
            {
                Number = 89111,
                Name = "1 תכנות מתקדם",
                Year = 2021,
                ID = "C-89111-2021"
            },
            new Course
            {
                Number = 89112,
                Name = "מבוא לרשתות תקשורת",
                Year = 2021,
                ID = "C-89112-2021"
            }
        };
        public static readonly List<ExerciseFull> ExerciseList = new List<ExerciseFull>
        {
            new ExerciseFull
            {
                ID = "89-111-2020-1",
                Name = "Ex1",
                Dates = new List<SubmitDate>
                {
                    new SubmitDate
                    {
                        date = DateTime.Now.Date,
                        ID = "a"
                    }
                },
                MaxSubmitters = 1
            },
            new ExerciseFull
            {
                ID = "89-111-2020-2",
                Name = "Ex2",
                Dates = new List<SubmitDate>
                {
                    new SubmitDate {
                        date = DateTime.Now.Date,
                        ID = "b"
                    }
                },
                MaxSubmitters = 1
            }
        };
        public static readonly List<SubmissionData> Submissions = new List<SubmissionData>
        {
            new SubmissionData
            {
                ID = "89111-2021-1-111111111",
                TotalGrade = 100,
                ManualGrade = 100,
                AutoGrade = 100,
                StyleGrade = 100,
                DateSubmitted = DateTime.Now,
                Folder = SubmitFile.Create("Submissions/"),
                State = SubmissionState.Checked,
                Submitters = new List<Student>
                {
                    new Student
                    {
                        Name = "aa bb",
                        ID = "111111111"
                    }
                }
            },
            new SubmissionData
            {
                ID = "89111-2021-2-111111111",
                TotalGrade = -1,
                ManualGrade = -1,
                AutoGrade = -1,
                StyleGrade = -1,
                DateSubmitted = DateTime.Now,
                State = SubmissionState.Unchecked,
                Submitters = new List<Student>
                {
                    new Student
                    {
                        Name = "aa bb",
                        ID = "111111111"
                    }
                }
            }
        };
        public static readonly Message Msg = new Message
        {
            ID = "1",
            SenderID = "111111111",
            Date = DateTime.Now,
            Body = "שלום",
            ChatID = "1"
            
        };

    }
}
