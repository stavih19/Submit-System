using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Submit_System {
    public enum ChatType { Extension, Appeal }
    public enum ChatState { Open, Rejected, Accepted }
    public class Chat {
        public string ID {get; set; }
        public ChatType Type { get; set; }
        public bool IsClosed { get; set; } = false;
        public ChatState State { get; set; } = ChatState.Open;
        [JsonIgnore]
        public string SubmissionID { get; set; }

        public Chat(){}
        public Chat(string submissionId, ChatType type){
            this.SubmissionID = submissionId;
            this.Type = type;
            this.ID = GenerateID(submissionId, type);
        }
         public string GenerateID(string submission_id, ChatType type){
            char c = 'A';
            c += (char) type;
            return ""+c+submission_id;
        }
    }
}