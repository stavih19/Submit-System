using System.Collections.Generic;
using System;
namespace Submit_System {
    public class Message {
        public string ID { get; set; }
        public string SenderID { get; set; }
        public string SenderName { get; set; }
        public DateTime Date { get; set; }
        public string Body { get; set; }
        public string ChatID { get; set; } 
        public int Status {get; set; }
        public bool IsFile {get; set; }
        public string FilePath {
            set
            {
                
            }
        }
        public SubmitFile file { get; set; } = null;
        public bool IsTeacher { get; set; }

        // public static Message Convert(MessageData data)
        // {
        //     if(data == null) {return null;}
        //     return new Message {
        //         SenderID = data.Sender_ID,
        //         SenderName = data.Course_ID,
        //         Date = data.Time,
        //         Body = data.Value,
        //         ChatID = data.Chat_ID,
        //         Status = data.Status,
        //         Type = data.Type,
        //         IsTeacher = false
        //     };
        // }
        // public static List<Message> Convert(List<MessageData> data)
        // {
        //     if(data == null) {return null;}
        //     var messages = new List<Message>();
        //     for(int i = 0; i < data.Count; i++)
        //     {
        //         if(data[i].Type == 1)
        //         {
        //             messages[messages.Count - 1].file = SubmitFile.Create(data[i].Value);
        //         }
        //         else
        //         {
        //             messages.Add(Message.Convert(data[i]));
        //         }
        //     }
        //     foreach(var d in data)
        //     {
        //         messages.Add(Convert(d));
        //     }
        //     return messages;
        // }
    }
}