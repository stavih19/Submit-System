using System.Collections.Generic;
namespace Submit_System {
    public class Course {

        public static  string GenerateID(int number,string name,int year,int semester){
            return year.ToString()+semester.ToString()+number.ToString();
        }

        public Course(){}

        public Course(int number,string name,int year,int semester){
            this.ID = Course.GenerateID(number, name,year,semester);;
            this.Number = number;
            this.Name = name;
            this.Year =year;
            this.Semester = semester;
        }
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
