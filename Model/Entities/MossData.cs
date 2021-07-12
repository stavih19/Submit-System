using System.Text.Json.Serialization;
namespace Submit_System
{
    /// <summary>
    ///     Includes the parameters to send to the moss server.
    /// </summary>
    public class MossData
    {
        public string Comment { get; set; } = "";
        public string ExerciseID { get; set; }
        public string Result {get; set;}
        public int MaxFound { get; set; } = MossClient.DEF_MAX_FOUND;
        public int MatchesShow { get; set; } = MossClient.DEF_SHOW;
        public string Language { get; set; }
        public bool IsExperimental {get; set;} = false;
        [JsonIgnore]
        public string SubmissionsFolder { get; set; } = null;
        [JsonIgnore]
        public string BaseFilesFolder { get; set; } = null;

    }
}