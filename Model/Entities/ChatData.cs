namespace Submit_System {
    public class ChatData{
        public ChatData(string id,string submission_id,int status,int type){
            this.ID = id;
            this.Submission_ID = submission_id;
            this.Status = status;
            this.Type = type;
        }
        public string ID{get;set;}
        public string Submission_ID{get;set;}
        public int Status{get;set;}
        public int Type{get;set;}

        public string GenerateID(string submission_id,int type){
            char c = 'A';
            c += (char) type;
            return ""+c+submission_id;
        }
    }
}