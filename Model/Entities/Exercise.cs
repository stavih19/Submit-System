using System.Collections.Generic;
namespace Submit_System {
    public class Exercise {

        public void GenerateID(){
            this.ID = this.Course_ID + "|" + this.Name;
        }

        public string ID {get; set; }
        public string Name {get; set; }
        
        public string Course_ID { get; set; }

        public string Original_Exercise_ID{ get; set; }

        public int Max_Submitters { get; set; }

        public string Test_Files_Location { get; set; }

        public string Late_Submittion_Settings { get; set; }
        public string Programming_Language { get; set; }
        public int Auto_Test_Grade_Value { get; set; }
        public int Style_Test_Grade_Value { get; set; }
        public int Is_Active { get; set; }
    }
}