using System.Text.Json;
namespace Submit_System
{
    /// <summary>
    ///     Includes the parameters to send to the moss server.
    /// </summary>
    public class MossData
    {
        public string Comment { get; set; } = "";
        public string ExcerciseID { get; set; } = null;
        public int MaxFound { get; set; } = MossClient.DEF_MAX_FOUND;
        public int MatchesShow { get; set; } = MossClient.DEF_SHOW;
        public string Language { get; set; }
        public bool IsExperimental {get; set;} = false;
        public string SubmissionsFolder { get; set; } = null;
        public string BaseFilesFolder { get; set; } = null;

    }
}