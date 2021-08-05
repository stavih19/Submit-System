using System.Collections.Generic;
using System;
namespace Submit_System
{
    public class ExerciseInput
    {
        public Exercise Exercise { get; set; }
         public DateTime? MainDate { get; set; }
        public List<SubmitFile> HelpFiles { get; set; }
    }
}