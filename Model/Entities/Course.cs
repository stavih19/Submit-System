using System.Collections.Generic;
namespace Submit_System {
    public class Course {

        public Course(){}
        public Course(string id,int number,string name,int year,int semester){
            this.ID = id;
            this.Number = number;
            this.Name = name;
            this.Year =year;
            this.Semester = semester;
        }
        public string ID {get; set; }

        public int Number {get; set; }
        
        public string Name {get; set; }
        
        public int Year { get; set; }

        public int Semester{ get; set; }

    }
}