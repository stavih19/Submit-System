using System;
namespace Submit_System {
    public class MessageData{
        public MessageData(string chat_id,DateTime time,int type,string value,string sender_id,int status,string course_id){
            this.Chat_ID = chat_id;
            this.Time = time;
            this.Type = type;
            this.Value = value;
            this.Sender_ID = sender_id;
            this.Status = status;
            this.Course_ID= course_id;
        }
        public MessageData() {}
        public string Chat_ID{get;set;}

        public DateTime Time{get;set;}

        public int Type{get;set;}

        public string Value{get;set;}

        public string Sender_ID{get;set;}
        public int Status{get;set;}

        public string Course_ID{get;set;}
    }
}
