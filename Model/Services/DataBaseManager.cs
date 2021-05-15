using System;
using System.Data.SqlClient;

namespace Submit_System
{
    public class DataBaseManager{
        private static string connetionString = "Data Source=127.0.0.1;Initial Catalog=submit02;User ID=SA;Password=HKCGHFKJ,GKG56fvfviW";

        public static (User,int,string) ReadUser(string id) {
            if(!User.IsValidID(id)){
                return (null,2,"Invalid id");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Users WHERE user_id = @ID;";
            User  u;
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    return (null,1,"User does not exist");
                }
                u = new User(dataReader.GetValue(0).ToString(),dataReader.GetValue(1).ToString(),dataReader.GetValue(2).ToString(),dataReader.GetValue(3).ToString());
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (u,4,"Connection close failed");}
            return (u,0,"OK");   
        }

        public static (int,string) AddUser(User user){
            (User u,int err,String s) = DataBaseManager.ReadUser(user.ID);
            if(u != null){
                return (1,"User is already exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Users VALUES (@ID, @PWD, @NAME,@EMAIL);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@PWD",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@NAME",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@EMAIL",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = user.ID;
            command.Parameters["@PWD"].Value = user.PASSWORD_HASH;
            command.Parameters["@NAME"].Value = user.NAME;
            command.Parameters["@EMAIL"].Value = user.EMAIL;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (Course,int,string) ReadCourse(string id) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Courses WHERE course_id = @ID;";
            Course  c;
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    return (null,1,"Course does not exist");
                }
                c = new Course(dataReader.GetValue(0).ToString(),(int) dataReader.GetValue(1),dataReader.GetValue(2).ToString(),(int)dataReader.GetValue(3),(int)dataReader.GetValue(4));
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (c,4,"Connection close failed");}
            return (c,0,"OK");   
        }

        public static (int,string) AddCourse(Course course){
            (Course c,int err,String s) = DataBaseManager.ReadCourse(course.ID);
            if(c != null){
                return (1,"Course is already exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Courses VALUES (@ID, @NUMBER, @NAME,@YEAR,@SEMESTER);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@NUMBER",System.Data.SqlDbType.Int);
            command.Parameters.Add("@NAME",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@YEAR",System.Data.SqlDbType.Int);
            command.Parameters.Add("@SEMESTER",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = course.ID;
            command.Parameters["@NUMBER"].Value = course.Number;
            command.Parameters["@NAME"].Value = course.Name;
            command.Parameters["@YEAR"].Value = course.Year;
            command.Parameters["@SEMESTER"].Value = course.Semester;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }





        
    }

    
}
