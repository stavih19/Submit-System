using System.Collections.Generic;
using System;
namespace Submit_System {
    /// <summary>
    ///     Fake Database for testing purposes
    /// </summary>
    public static class FakeDatabase {
        public const string C1ID = "89111-2021";
        public const string C2ID = "89112-2021";
        public const string Ex1ID = C1ID + "-1";
        public const string Ex2ID = C1ID + "-2";
        public const string S1ID = Ex1ID + "-111111111";
        public const string S2ID = Ex2ID + "-111111111";

        public static readonly List<Course> CourseList = new List<Course>
        {
            new Course
            {
                Number = 89111,
                Name = "1 תכנות מתקדם",
                Year = 2021,
                ID = C1ID
            },
            new Course
            {
                Number = 89112,
                Name = "מבוא לרשתות תקשורת",
                Year = 2021,
                ID = C2ID
            }
        };
        public static readonly List<ExerciseFull> ExerciseList = new List<ExerciseFull>
        {
            new ExerciseFull
            {
                ID = Ex1ID,
                Name = "Ex1",
                Dates = new List<SubmitDate>
                {
                    new SubmitDate
                    {
                        date = DateTime.Now.Date,
                        ID = "a"
                    }
                },
                MaxSubmitters = 1,
                CourseID = C1ID
            },
            new ExerciseFull
            {
                ID = Ex2ID,
                Name = "Ex2",
                Dates = new List<SubmitDate>
                {
                    new SubmitDate {
                        date = DateTime.Now.Date,
                        ID = "b"
                    }
                },
                MaxSubmitters = 1,
                CourseID = C1ID
                
            }
        };
        public static readonly List<SubmissionData> Submissions = new List<SubmissionData>
        {
            new SubmissionData
            {
                ID = S1ID,
                ExID = Ex1ID,
                TotalGrade = 100,
                ManualGrade = 100,
                AutoGrade = 100,
                StyleGrade = 100,
                DateSubmitted = DateTime.Now,
                State = SubmissionState.Checked,
                Folder = "Courses\\89111\\Exercises\\Ex1\\Submissions\\Yosi",
                Submitters = new List<Student>
                {
                    new Student
                    {
                        Name = "Yosi",
                        ID = "111111111"
                    }
                },
                ExtensionChat = new Chat
                {
                    ID = "bbb_ext",
                    IsClosed = true,
                    Type = ChatType.Extension

                },
                AppealChat = new Chat
                {
                    ID = "bbb_app",
                    IsClosed = false,
                    Type = ChatType.Appeal

                }
            },
            new SubmissionData
            {
                ID = S2ID,
                ExID = Ex2ID,
                TotalGrade = -1,
                ManualGrade = -1,
                AutoGrade = -1,
                StyleGrade = -1,
                DateSubmitted = DateTime.MinValue,
                State = SubmissionState.Unchecked,
                Folder = "Courses\\89111\\Exercises\\Ex2\\Submissions\\Yosi",
                Submitters = new List<Student>
                {
                    new Student
                    {
                        Name = "Yosi",
                        ID = "111111111"
                    }
                },
                ExtensionChat = new Chat
                {
                    ID = "aaa_ext",
                    IsClosed = false,
                    Type = ChatType.Extension
                }
            }
        };
        public static readonly Message Msg = new Message
        {
            ID = "1",
            SenderID = "111111111",
            Date = DateTime.Now,
            Body = "שלום",
            ChatID = "1",
            IsTeacher = false,
            SenderName = "Yosi Yosi"
            
        };
         public static readonly Message Msg2 = new Message
        {
            ID = "2",
            SenderID = "111115115",
            Date = DateTime.Now,
            Body = ".שלום",
            ChatID = "1",
            IsTeacher = true,
            SenderName = "Avi Avi"
            
        };

    }
}
