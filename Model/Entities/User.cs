namespace Submit_System {
    class User{
        public User(string id,string name,string password_hash,string email){
            this.ID = id;
            this.NAME = name;
            this.PASSWORD_HASH = password_hash;
            this.EMAIL = email;
        }
        public string ID{get;set;}
        public string PASSWORD_HASH{get;set;}
        public string NAME{get;set;}
        public string EMAIL{get;set;}
    }
}