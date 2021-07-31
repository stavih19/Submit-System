using System.Collections.Generic;
namespace Submit_System
{
    public class TestOutput
    {
        public int ID { get; set; }
        public List<string> Files { get; set;}
    }
    public class ExOutput
    {
        public string ID { get; set; }
        public List<string> Files { get; set;}
    }
    public class SubmitOutput
    {
        public string Text { get; set; }
        public int AutoGrade { get; set; }
        public int StyleGrade { get; set; }
    }
}