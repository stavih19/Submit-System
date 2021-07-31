namespace Submit_System {
    public interface OutputComperator
    {
        public int CompareOutput(string expected_output,string output);
    }

    public class BasicOutputComperator : OutputComperator
    {
        public int CompareOutput(string expected_output, string output)
        {
            if(expected_output == output){
                return 100;
            }
            return 0;
        }
    }
}