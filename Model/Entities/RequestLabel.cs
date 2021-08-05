using System;
using System.Collections.Generic;
namespace Submit_System {
    public class RequestLabel {      
        public string ChatID { get; set; }
        public string StudentID {get; set; }
        public string StudentName {get; set; }
        public string Message {get; set;}
        public ChatType Type { get; set; }
    }
    public class RequestLabelMainPage {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public int CourseNumber { get; set; }
        public string ExerciseID { get; set; }
        public string ExerciseName { get; set; }
        public string ChatID { get; set; }
        public string SubmissionID { get; set; }
        public string StudentName {get; set; }
        public ChatType Type { get; set; }
    }
}
