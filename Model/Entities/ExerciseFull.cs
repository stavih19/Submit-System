using System;
using System.Collections.Generic;
namespace Submit_System {
    /// <summary>
    /// Represents all Excercise de
    /// </summary>
    public class ExerciseFull {
        public string ID {get; set; }
        public string Name { get; set; }
        public List<SubmitDate> Dates {get; set; }
        public bool IsMultipleSubmission { get; set; }
        public string Language { get; set; }
        public int ManualGradeWeight { get; set; }
        public int StyleGradeWeight { get; set; }
        public int AutoGradeWeight { get; set; }
        public bool IsActive { get; set; }
        public int MaxSubmitters { get; set; }
    }
}
