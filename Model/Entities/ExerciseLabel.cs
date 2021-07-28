using System;
using System.Collections.Generic;
namespace Submit_System {
    public class ExerciseLabel {
        
        public string ID {get; set; }
        public string Name { get; set; }
        public ExerciseLabel(string id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
