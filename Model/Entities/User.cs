namespace Submit_System {
    public class User{
        public User(string id,string password_hash,string name,string email){
            this.ID = id;
            this.PASSWORD_HASH = password_hash;
            this.NAME = name;
            this.EMAIL = email;
        }
        public string ID{get;set;}
        public string PASSWORD_HASH{get;set;}
        public string NAME{get;set;}
        public string EMAIL{get;set;}

        public static bool IsValidID(string id){
            return (id.Length <= 32) && int.TryParse(id,out _);
        }
    }
}