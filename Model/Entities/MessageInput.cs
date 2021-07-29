using System.Text.Json.Serialization;
namespace Submit_System {
    public class MessageInput
    {
        public string ChatID {get; set;}
        public string Text { get; set; }
        public SubmitFile AttachedFile {get; set;}
        [JsonIgnore]
        public string FilePath { get; set; }
    }
}