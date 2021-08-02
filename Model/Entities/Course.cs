namespace Submit_System {
    public class Course {

        public static  string GenerateID(int number,int year){
            return number + "_" + year + "_";
        }
        public void GenerateID()
        {
            ID = GenerateID(this.Number, this.Year);
        }
        public Course(){}

        public Course(int number,string name,int year,int semester){
            this.ID =  GenerateID(number, year);
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
