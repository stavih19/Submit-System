using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;
namespace Submit_System {
    public class Message {
        public int ID { get; set; }
        public string SenderID { get; set; }
        public string SenderName { get; set; }
        public DateTime Date { get; set; }
        public string Body { get; set; }
        public string ChatID { get; set; } 
        public int Status {get; set; }
        [JsonIgnore]
        public string AttachedFilePath { get; set; }
        public string Filename { get => (AttachedFilePath == null) ? null : FileUtils.GetFileName(AttachedFilePath); }
        public bool IsTeacher { get; set; }
    
    }
}