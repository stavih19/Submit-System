using System;
using System.Collections.Generic;
namespace Submit_System {
    public class ExerciseDetails {
        
        public string ID {get; set; }
        public List<SubmitDate> Dates {get; set; }
        public bool IsMultipleSubmitters { get; set; }
        public string Language { get; set; }
        public int ManualGradeWeight { get; set; }
        public int StyleGradeWeight { get; set; }
        public int OutputGradeWeight { get; set; }
    }
}