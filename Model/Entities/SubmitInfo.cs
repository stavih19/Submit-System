using System.Collections.Generic;
namespace Submit_System
{
    public class SubmitInfo
    {
        public string SubmissionID { get; set; }
        public string Path { get; set; }
        public string ExerciseID { get; set; }
        public string ExerciseName { get; set; }
        public string CourseID { get; set; }
        public string ProgrammingLanguage { get; set; }
        public List<Test> Tests { get; set; }
        public string Text { get; set; }
    }
}