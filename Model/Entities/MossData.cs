using System.Text.Json.Serialization;
using System.Text.Json;
namespace Submit_System
{
    /// <summary>
    ///     Includes the parameters to send to the moss server.
    /// </summary>
    public class MossData
    {
        public string Comment { get => $"Exercise {ExerciseID ?? ""}"; }
        public string ExerciseID { get; set; }
        public string Result {get; set;}
        public int MaxFound { get; set; } = MossClient.DEF_MAX_FOUND;
        public int MatchesShow { get; set; } = MossClient.DEF_SHOW;
        [JsonIgnore]
        public string Language { get; set; }
        [JsonIgnore]
        public bool IsExperimental {get; set;} = false;
        [JsonIgnore]
        public string SubmissionsFolder { get; set; } = null;
        [JsonIgnore]
        public string BaseFilesFolder { get; set; } = null;

    }
}