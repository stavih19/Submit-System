using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Web;

namespace Submit_System {
    public class ExcelLoader {
        private static List<string[]> ReadExcel(string file_name){
            using (TextFieldParser parser = new TextFieldParser(file_name))
            {
                List<string[]> lst = new List<string[]>();
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                //ignores the first line
                if(!parser.EndOfData){
                    parser.ReadFields();
                }
                while (!parser.EndOfData)
                {
                    lst.Add(parser.ReadFields());
                }
                return lst;
            }
        }
        public static string ReadUsersFromFile(string input_file_name,DatabaseAccess _access){
            List<string[]> users;
            string output = "";
            try{
                users = ReadExcel(input_file_name);
            } catch {
                return "Can't load the file "+input_file_name;
            }
            foreach(string[] user_data in users){
                string error_msg = "Added";
                User u = new User(user_data[0],null,user_data[2]+' '+user_data[1],user_data[11]);
                int err = DataBaseManager.AddUser(u).Item1;
                if(err == 0){
                    PasswordRequest(u.ID, u.Email,_access);
                }
                if(err == 1){
                    error_msg = "User is already exists";
                }
                if(err == 2 || err == 3){
                    error_msg = "Connection Error: try again";
                }
                string line = u.ID+','+u.Name+','+error_msg+'\n';
                output = output+line;
            }
            return output;
        }

        private static bool PasswordRequest(string userID, string email,DatabaseAccess _access)
        {
            string token = CryptoUtils.GetRandomBase64String(24);
            string tokenHash = CryptoUtils.Sha256Hash(token);
            string link =  String.Format(UserController.PASSWORD_LINK, HttpUtility.UrlEncode(token));
            DBCode code = _access.AddPasswordToken(userID, tokenHash, DateTime.Now.AddDays(2));
            if(code != DBCode.OK)
            {
                return false;
            }
            string text = $"Password form link:<br>" + link;
            MaleUtils.SendMail(email, "Submit Bar Ilan User registration" ,text);
            return true;
        }

    }

}