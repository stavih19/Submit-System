using System;
using System.Data.SqlClient;
using System.Collections.Generic;

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
                    try{cnn.Close();}catch{}
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
                    try{cnn.Close();}catch{}
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

        public static (bool,int,string) IsStudentInCourse(string course_id,string student_id){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (false,3,"Connection failed");}
            String sql = "SELECT * FROM Student_Course WHERE course_id = @CID AND user_id = @UID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.VarChar);
            command.Parameters["@UID"].Value = student_id;
            command.Parameters["@CID"].Value = course_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return (false,0,"OK");
                }
            } catch {
                try{cnn.Close();}catch{}
                return (false,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (true,4,"Connection close failed");}
            return (true,0,"OK"); 
        }

        public static (int,string) AddStudentToCourse(string course_id,string student_id){
            (bool exist,int err,String s) = DataBaseManager.IsStudentInCourse(course_id,student_id);
            if(exist){
                return (1,"The Student is already in the course");
            }
            if(err == 3){
                return (err,s);
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Student_Course VALUES (@UID, @CID);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.VarChar);
            command.Parameters["@UID"].Value = student_id;
            command.Parameters["@CID"].Value = course_id;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (List<string>,int,string) ReadCoursesIdOfStudent(string student_id){
            List<string> lst = new List<string>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT course_id FROM Student_Course WHERE user_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = student_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add(dataReader.GetValue(0).ToString());
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }

        public static (List<string>,int,string) ReadStudentsIdOfCourse(string course_id){
            List<string> lst = new List<string>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT user_id FROM Student_Course WHERE course_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = course_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add(dataReader.GetValue(0).ToString());
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }

        public static (Exercise,int,string) ReadExercise(string id) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Exercise WHERE exercise_id = @ID;";
            Exercise  ex;
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return (null,1,"Exercise does not exist");
                }
                ex = new Exercise();
                ex.ID = dataReader.GetValue(0).ToString();
                ex.Name = dataReader.GetValue(1).ToString();
                ex.Course_ID = dataReader.GetValue(2).ToString();
                ex.Original_Exercise_ID = dataReader.GetValue(3).ToString();
                ex.Max_Submitters = (int)dataReader.GetValue(4);
                ex.Test_Files_Location = dataReader.GetValue(5).ToString();
                ex.Late_Submittion_Settings = dataReader.GetValue(6).ToString();
                ex.Programming_Language = dataReader.GetValue(7).ToString();
                ex.Auto_Test_Grade_Value = (int)dataReader.GetValue(8);
                ex.Style_Test_Grade_Value = (int)dataReader.GetValue(9);
                ex.Is_Active = (int)dataReader.GetValue(10);
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (ex,4,"Connection close failed");}
            return (ex,0,"OK");   
        }

        public static (int,string) AddExercise(Exercise exercise){
            (Exercise e,int err,String s) = DataBaseManager.ReadExercise(exercise.ID);
            if(e != null){
                return (1,"Exercise is already exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Exercise VALUES (@ID,@NAME,@CID,@OEID,@MAX,@FILES,@LATEST,@LANG,@AUTO,@STYLE,@ACTIVE);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@NAME",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@OEID",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@MAX",System.Data.SqlDbType.Int);
            command.Parameters.Add("@FILES",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@LATEST",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@LANG",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@AUTO",System.Data.SqlDbType.Int);
            command.Parameters.Add("@STYLE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@ACTIVE",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = exercise.ID;
            command.Parameters["@NAME"].Value = exercise.Name;
            command.Parameters["@CID"].Value = exercise.Course_ID;
            command.Parameters["@OEID"].Value = exercise.Original_Exercise_ID;
            command.Parameters["@MAX"].Value = exercise.Max_Submitters;
            command.Parameters["@FILES"].Value = exercise.Test_Files_Location;
            command.Parameters["@LATEST"].Value = exercise.Late_Submittion_Settings;
            command.Parameters["@LANG"].Value = exercise.Programming_Language;
            command.Parameters["@AUTO"].Value = exercise.Auto_Test_Grade_Value;
            command.Parameters["@STYLE"].Value = exercise.Style_Test_Grade_Value;
            command.Parameters["@ACTIVE"].Value = exercise.Is_Active;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (List<string>,int,string) ReadExercisesIdOfCourse(string course_id){
            List<string> lst = new List<string>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT exercise_id FROM Exercise WHERE course_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = course_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add(dataReader.GetValue(0).ToString());
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }






    }

    
}
