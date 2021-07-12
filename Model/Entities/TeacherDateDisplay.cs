using System;
using System.Collections.Generic;
namespace Submit_System {
    public class TeacherDateDisplay {
        public int  ID { get; set; }
        public DateTime Date { get; set;}
        public int Group {get; set; }
        public int Reduction { get; set; }
        public List<UserLabel> submitters {get; set; }

    }
}
