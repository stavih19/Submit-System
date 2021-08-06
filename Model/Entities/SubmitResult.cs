using System.Collections.Generic;
namespace Submit_System {
   public class SubmitResult
   {
       public string Message {get; set;}
       public List<string> Files { get; set; }
        public int AutoGrade { get; set; }
        public int StyleGrade { get; set; }
   }
}