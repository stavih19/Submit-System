using System.Text.Json.Serialization;
namespace Submit_System {
    public class User{
        public User(string id,string password_hash,string name,string email){
            this.ID = id;
            this.PasswordHash = password_hash;
            this.Name = name;
            this.Email = email;
        }
        public User() {}
        public string ID{get;set;}
        [JsonIgnore]
        public string PasswordHash{get;set;}
        public string Name{get;set;}
        public string Email{get;set;}

        public static bool IsValidID(string id){
            return (id.Length <= 10);
        }
    }
}