using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Web;

namespace Submit_System {
    public class ExcelLoader {

        private static Dictionary<string,int> SEMESTERS;
        static ExcelLoader(){
            SEMESTERS = new Dictionary<string, int>();
            SEMESTERS.Add("סמסטר א'",1);
            SEMESTERS.Add("סמסטר ב'",2);
            SEMESTERS.Add("סמסטר קיץ",3);
            SEMESTERS.Add("שנתי",4);
        }
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
                if(err == 0|| err==4){
                    bool ok =PasswordRequest(u.ID, u.Email,_access);
                    if(!ok){
                        error_msg = "Added,Failed to send email";
                    }
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

        public static string ReadCoursesFromFile(string input_file_name){
            List<string[]> courses_data;
            string output = "";
            try{
                courses_data = ReadExcel(input_file_name);
            } catch {
                return "Can't load the file "+input_file_name;
            }
            Dictionary<string,(Course,HashSet<string>)> courses= new Dictionary<string, (Course, HashSet<string>)>();
            foreach(string[] cours_data in courses_data){
                int course_number = CourseNumber(cours_data[0]);
                Course c = new Course(course_number,cours_data[1],int.Parse(cours_data[4]),Semester(cours_data[3]));
                if(courses.ContainsKey(c.ID)){
                    courses[c.ID].Item2.Add(cours_data[2]);
                } else{
                    HashSet<string> s = new HashSet<string>();
                    s.Add(cours_data[2]);
                    courses.Add(c.ID,(c,s));
                }
            }
            foreach(string cid in courses.Keys){
                Course c = courses[cid].Item1;
                int err =DataBaseManager.AddCourse(courses[cid].Item1).Item1;
                if(err == 2 || err == 3){
                    output = output + "FAILED to add the course "+c.Name+' '+c.Number+" - connection error,try again.\n";
                    continue;
                }
                if(err==0||err==4){
                    output = output + "Added the course "+c.Name+' '+c.Number+" With Teachers:\n";
                }
                if(err == 1){
                    output = output + "Course already exists "+c.Name+' '+c.Number+" Adding Teachers:\n";
                }
                foreach(string teacher in courses[cid].Item2){
                    err =DataBaseManager.AddMetargelToCourse(cid,teacher).Item1;
                    if(err == 2 || err == 3){
                        output = output + " * FAILED to add the teacher "+teacher+" - connection error,try again.\n";
                    }
                    if(err==0||err==4){
                        output = output + " * Added the teacher "+teacher+"\n";
                    }
                    if(err == 1){
                        output = output + " * The teacher "+teacher+" is already exists in this course\n";
                    }
                }
            }
            return output;
        }
        private static int Semester(string txt){
            try{
                return SEMESTERS[txt];
            }catch{
                return 0;
            }
        }

        private static int CourseNumber(string num){
            return int.Parse(num.Split('-')[0]);
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

        public static string AddStudentsToCourse(string input_file_name,string course_id){
            List<string[]> students;
            string output = "Adding students to the course:\n";
            try{
                students = ReadExcel(input_file_name);
            } catch {
                return "Can't load the file "+input_file_name;
            }
            foreach(string[] student_data in students){
                string error_msg = "Added";
                int err = DataBaseManager.AddStudentToCourse(course_id,student_data[0]).Item1;
                if(err == 1){
                    error_msg = "User is already exists";
                }
                if(err == 2 || err == 3){
                    error_msg = "Connection Error: try again";
                }
                string line = error_msg+' '+student_data[0]+'\n';
                output = output+line;
            }
            return output;
        }
    }

}