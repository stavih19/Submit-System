namespace Submit_System {
    public class ChatData{
        public ChatData(string id,string submission_id,int status,int type){
            this.ID = id;
            this.SubmissionID = submission_id;
            this.Status = status;
            this.Type = type;
        }
        public ChatData(string submission_id,int type){
            this.SubmissionID = submission_id;
            this.Type = type;
            this.ID = GenerateID(submission_id, type);
        }
        public string ID{get;set;}
        public string SubmissionID{get;set;}
        public int Status{get;set;}
        public int Type{get;set;}

        public string GenerateID(string submission_id,int type){
            char c = 'A';
            c += (char) type;
            return ""+c+submission_id;
        }
    }
}
