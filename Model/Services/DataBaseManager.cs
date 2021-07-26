using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Submit_System
{
    public partial class DataBaseManager{
        private static string connetionString = "Data Source=127.0.0.1;Initial Catalog=submit02;User ID=SA;Password=HKCGHFKJ,GKG56fvfviW";

        private static int WAITING_TO_METARGEL = 0;
        public static (User,int,string) ReadUser(string id) {
            if(!User.IsValidID(id)){
                return (null,2,"Invalid id");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Users WHERE user_id = @ID;";
            User  u;
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
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
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@PWD",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@NAME",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@EMAIL",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = user.ID;
            command.Parameters["@PWD"].Value = user.PasswordHash;
            command.Parameters["@NAME"].Value = user.Name;
            command.Parameters["@EMAIL"].Value = user.Email;
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
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
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
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@NUMBER",System.Data.SqlDbType.Int);
            command.Parameters.Add("@NAME",System.Data.SqlDbType.NVarChar);
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
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
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
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
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
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
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
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
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
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
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
                ex.MaxSubmitters = (int)dataReader.GetValue(4);
                ex.FilesLocation = dataReader.GetValue(5).ToString();
                ex.LateSubmissionSettings = dataReader.GetValue(6).ToString();
                ex.ProgrammingLanguage = dataReader.GetValue(7).ToString();
                ex.AutoTestGradeWeight = (int)dataReader.GetValue(8);
                ex.StyleTestGradeWeight = (int)dataReader.GetValue(9);
                ex.IsActive = (int)dataReader.GetValue(10);
                ex.MultipleSubmission = (int)dataReader.GetValue(11) == 1;
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
            String sql = "INSERT INTO Exercise VALUES (@ID,@NAME,@CID,@OEID,@MAX,@FILES,@LATEST,@LANG,@AUTO,@STYLE,@ACTIVE,@MULT);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@NAME",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@OEID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@MAX",System.Data.SqlDbType.Int);
            command.Parameters.Add("@FILES",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@LATEST",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@LANG",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@AUTO",System.Data.SqlDbType.Int);
            command.Parameters.Add("@STYLE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@ACTIVE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MULT",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = exercise.ID;
            command.Parameters["@NAME"].Value = exercise.Name;
            command.Parameters["@CID"].Value = exercise.Course_ID;
            command.Parameters["@OEID"].Value = exercise.Original_Exercise_ID;
            command.Parameters["@MAX"].Value = exercise.MaxSubmitters;
            command.Parameters["@FILES"].Value = exercise.FilesLocation;
            command.Parameters["@LATEST"].Value = exercise.LateSubmissionSettings;
            command.Parameters["@LANG"].Value = exercise.ProgrammingLanguage;
            command.Parameters["@AUTO"].Value = exercise.AutoTestGradeWeight;
            command.Parameters["@STYLE"].Value = exercise.StyleTestGradeWeight;
            command.Parameters["@ACTIVE"].Value = exercise.IsActive;
            command.Parameters["@MULT"].Value = exercise.MultipleSubmission ? 1 : 0;
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
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
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

        public static (Dictionary<int,DateTime>,int,string) ReadDatseOfExercise(string exercise_id){
            Dictionary<int,DateTime> dict = new Dictionary<int, DateTime>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Submission_Dates WHERE exercise_id = @ID;";
            DateTime date;
            int date_id;
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = exercise_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    date_id = (int)dataReader.GetValue(1);
                    date = (DateTime)dataReader.GetValue(2);
                    dict[date_id] = date;
                }
            } catch{
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (dict,4,"Connection close failed");}
            return (dict,0,"OK");   
        }

        public static (int,string) AddDateToExercise(string exercise_id,int date_id,DateTime date){
            (Dictionary<int,DateTime> d,int err,String s) = DataBaseManager.ReadDatseOfExercise(exercise_id);
            if(d != null){
                if(d.ContainsKey(date_id)){
                    return (1,"Date id is already exist");
                }
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Submission_Dates VALUES (@EID, @DID, @DATE);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@EID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@DID",System.Data.SqlDbType.Int);
            command.Parameters.Add("@Date",System.Data.SqlDbType.Date);
            command.Parameters["@EID"].Value = exercise_id;
            command.Parameters["@DID"].Value = date_id;
            command.Parameters["@Date"].Value = date;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (Submission,int,string) ReadSubmission(string id) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Submission WHERE submission_id = @ID;";
            Submission  sub = new Submission();
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return (null,1,"Submission does not exist");
                }
                sub.ID = dataReader.GetValue(0).ToString();
                sub.Exercise_ID = dataReader.GetValue(1).ToString();
                sub.Files_Location = dataReader.GetValue(2).ToString();
                sub.Auto_Grade = (int)dataReader.GetValue(3);
                sub.Style_Grade = (int)dataReader.GetValue(4);
                sub.Manual_Final_Grade = (int)dataReader.GetValue(5);
                sub.Manual_Check_Data = dataReader.GetValue(6).ToString();
                sub.Submission_Status = (int)dataReader.GetValue(7);
                sub.Submission_Date_Id = (int)dataReader.GetValue(8);
                sub.Time_Submitted = (DateTime)dataReader.GetValue(9);
                sub.Chat_Late_ID = dataReader.GetValue(10).ToString();
                sub.Chat_Appeal_ID = dataReader.GetValue(11).ToString();
                sub.HasCopied = (int)dataReader.GetValue(12) == 1;
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (sub,4,"Connection close failed");}
            return (sub,0,"OK");   
        }

        public static (int,string) AddSubmission(Submission submission){
            (Submission sub,int err,String s) = DataBaseManager.ReadSubmission(submission.ID);
            if(sub != null){
                return (1,"Submission is already exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Submission VALUES (@ID,@EID,@FL,@AG,@SG,@MFG,@MD,@STATUS,@DID,@SUBMITTED,@CL,@CA,@HC);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@EID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@FL",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@AG",System.Data.SqlDbType.Int);
            command.Parameters.Add("@SG",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MFG",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MD",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@STATUS",System.Data.SqlDbType.Int);
            command.Parameters.Add("@DID",System.Data.SqlDbType.Int);
            command.Parameters.Add("@SUBMITTED",System.Data.SqlDbType.DateTime);
            command.Parameters.Add("@CL",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CA",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@HC",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = submission.ID;
            command.Parameters["@EID"].Value = submission.Exercise_ID;
            command.Parameters["@FL"].Value = submission.Files_Location;
            command.Parameters["@AG"].Value = submission.Auto_Grade;
            command.Parameters["@SG"].Value = submission.Style_Grade;
            command.Parameters["@MFG"].Value = submission.Manual_Final_Grade;
            command.Parameters["@MD"].Value = submission.Manual_Check_Data;
            command.Parameters["@STATUS"].Value = submission.Submission_Status;
            command.Parameters["@DID"].Value = submission.Submission_Date_Id;
            command.Parameters["@SUBMITTED"].Value = submission.Time_Submitted;
            command.Parameters["@CL"].Value = submission.Chat_Late_ID;
            command.Parameters["@CA"].Value = submission.Chat_Appeal_ID;
            command.Parameters["@HC"].Value = submission.HasCopied;
            try{
                command.ExecuteReader();
            } catch(Exception e){
                try{cnn.Close();}catch{}
                return (2,"Addittion failed "+e.Message);
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (int,int,string) ReadTypeOfStudentInSubmission(string submission_id,string student_id){
            int type;
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (-1,3,"Connection failed");}
            String sql = "SELECT submitter_type FROM Submitters WHERE submission_id = @SID AND user_id = @UID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = student_id;
            command.Parameters["@SID"].Value = submission_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return (0,0,"The student is not in this submission");
                }
                type = (int)dataReader.GetValue(0);
            } catch{
                try{cnn.Close();}catch{}
                return (-1,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (type,4,"Connection close failed");}
            return (type,0,"OK"); 
        }

        public static (int,string) AddStudentToSubmission(string submission_id,string student_id,int type,string exercise_id){
            (int t,int err,String s) = DataBaseManager.ReadTypeOfStudentInSubmission(submission_id,student_id);
            if(t>0){
                return (1,"The Student is already in the Submission");
            }
            if(err == 3){
                return (err,s);
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Submitters VALUES (@UID, @SID,@TYPE,@EID);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@EID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = student_id;
            command.Parameters["@SID"].Value = submission_id;
            command.Parameters["@TYPE"].Value = type;
            command.Parameters["@EID"].Value = exercise_id;
            try{
                command.ExecuteReader();
            } catch{
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static ((string,int),int,string) ReadSubmissionIdAndTypeOfStudentInExercise(string student_id,string exercise_id){
            string submission_id;
            int type;
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return ((null,-1),3,"Connection failed");}
            String sql = "SELECT submission_id,submitter_type FROM Submitters WHERE user_id = @UID AND exercise_id = @EID AND submitter_type != 3;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@EID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = student_id;
            command.Parameters["@EID"].Value = exercise_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return ((null,0),1,"The user has no submission at this exercise");
                }
                submission_id=dataReader.GetValue(0).ToString();
                type = (int)dataReader.GetValue(1);
            } catch{
                try{cnn.Close();}catch{}
                return ((null,-1),3,"Connection failed");
            }
            try{cnn.Close();} catch{return ((submission_id,type),4,"Connection close failed");}
            return ((submission_id,type),0,"OK");   
        }

        public static (List<(string,int)>,int,string) ReadStudentsIdAndTypeOfSubmission(string submission_id){
            List<(string,int)> lst = new List<(string,int)>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT user_id,submitter_type FROM Submitters WHERE submission_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = submission_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add((dataReader.GetValue(0).ToString(),(int)dataReader.GetValue(1)));
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }

        public static (int,string) UpdateExercise(Exercise exercise){
            (Exercise e,int err,String s) = DataBaseManager.ReadExercise(exercise.ID);
            if(err == 3){
                return (err,s);
            }
            if(e == null){
                return (1,"Exercise does not exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "UPDATE Exercise SET exercise_name=@NAME,course_id=@CID,original_exercise_id=@OEID,max_submitters=@MAX,test_files_location=@FILES,late_submittion_settings=@LATEST,programming_language=@LANG,auto_test_grade_value=@AUTO,style_test_grade_value=@STYLE,is_active=@ACTIVE,multiple_submissions=@MULT WHERE exercise_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@NAME",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@OEID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@MAX",System.Data.SqlDbType.Int);
            command.Parameters.Add("@FILES",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@LATEST",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@LANG",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@AUTO",System.Data.SqlDbType.Int);
            command.Parameters.Add("@STYLE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@ACTIVE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MULT",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = exercise.ID;
            command.Parameters["@NAME"].Value = exercise.Name;
            command.Parameters["@CID"].Value = exercise.Course_ID;
            command.Parameters["@OEID"].Value = exercise.Original_Exercise_ID;
            command.Parameters["@MAX"].Value = exercise.MaxSubmitters;
            command.Parameters["@FILES"].Value = exercise.FilesLocation;
            command.Parameters["@LATEST"].Value = exercise.LateSubmissionSettings;
            command.Parameters["@LANG"].Value = exercise.ProgrammingLanguage;
            command.Parameters["@AUTO"].Value = exercise.AutoTestGradeWeight;
            command.Parameters["@STYLE"].Value = exercise.StyleTestGradeWeight;
            command.Parameters["@ACTIVE"].Value = exercise.IsActive;
            command.Parameters["@MULT"].Value = exercise.MultipleSubmission;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        
        public static (List<string>,int,string) ReadJoinRequestsOfStudent(string student_id){
            List<string> lst = new List<string>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT submission_id FROM Submitters WHERE user_id = @ID AND submitter_type=3;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
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
    
        public static (int,string) DeleteStudentFromSubmission(string student_id,string submission_id,int type){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "DELETE FROM Submitters WHERE user_id = @UID AND submission_id= @SID AND submitter_type=@TYPE ;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters["@UID"].Value = student_id;
            command.Parameters["@SID"].Value = submission_id;
            command.Parameters["@TYPE"].Value = type;
            try{
                command.ExecuteReader();
            } catch{
                try{cnn.Close();}catch{}
                return (2,"Delete failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static (int,string) UpdateSubmission(Submission submission){
            (Submission sb,int err,String s) = DataBaseManager.ReadSubmission(submission.ID);
            if(err == 3){
                return (err,s);
            }
            if(sb == null){
                return (1,"Submission does not exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "UPDATE Submission SET exercise_id=@EID,files_location=@FL,auto_grade=@AG,style_grade=@SG,manual_final_grade=@MFG,manual_check_data=@MD,submission_status=@STATUS,submission_date_id=@DID,time_submitted=@SUBMITTED,chat_late_id=@CL,chat_appeal_id=@CA,has_copied=@HC WHERE submission_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@EID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@FL",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@AG",System.Data.SqlDbType.Int);
            command.Parameters.Add("@SG",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MFG",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MD",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@STATUS",System.Data.SqlDbType.Int);
            command.Parameters.Add("@DID",System.Data.SqlDbType.Int);
            command.Parameters.Add("@SUBMITTED",System.Data.SqlDbType.DateTime);
            command.Parameters.Add("@CL",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CA",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@HC",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = submission.ID;
            command.Parameters["@EID"].Value = submission.Exercise_ID;
            command.Parameters["@FL"].Value = submission.Files_Location;
            command.Parameters["@AG"].Value = submission.Auto_Grade;
            command.Parameters["@SG"].Value = submission.Style_Grade;
            command.Parameters["@MFG"].Value = submission.Manual_Final_Grade;
            command.Parameters["@MD"].Value = submission.Manual_Check_Data;
            command.Parameters["@STATUS"].Value = submission.Submission_Status;
            command.Parameters["@DID"].Value = submission.Submission_Date_Id;
            command.Parameters["@SUBMITTED"].Value = submission.Time_Submitted;
            command.Parameters["@CL"].Value = submission.Chat_Late_ID;
            command.Parameters["@CA"].Value = submission.Chat_Appeal_ID;
            command.Parameters["@HC"].Value = submission.HasCopied;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (ChatData,int,string) ReadChat(string id) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Chat WHERE chat_id = @ID;";
            ChatData  ch;
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return (null,1,"Chat does not exist");
                }
                ch = new ChatData(dataReader.GetValue(0).ToString(),dataReader.GetValue(1).ToString(),(int)dataReader.GetValue(2),(int)dataReader.GetValue(3));
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (ch,4,"Connection close failed");}
            return (ch,0,"OK");   
        }

        public static (int,string) AddChat(ChatData chat){
            (ChatData c,int err,String s) = DataBaseManager.ReadChat(chat.ID);
            if(err == 3){
                return (err,s);
            }
            if(c != null){
                return (1,"Chat is already exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Chat VALUES (@ID, @SID, @STATUS,@TYPE);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@STATUS",System.Data.SqlDbType.Int);
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = chat.ID;
            command.Parameters["@SID"].Value = chat.Submission_ID;
            command.Parameters["@STATUS"].Value = chat.Status;
            command.Parameters["@TYPE"].Value = chat.Type;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (MessageData,int,string) ReadMessage(int id) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Message WHERE message_id = @ID;";
            MessageData  msg;
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return (null,1,"Message does not exist");
                }
                msg = new MessageData(dataReader.GetValue(1).ToString(),(DateTime)dataReader.GetValue(2),(int)dataReader.GetValue(3),dataReader.GetValue(4).ToString(),dataReader.GetValue(5).ToString(),(int)dataReader.GetValue(6),dataReader.GetValue(7).ToString());
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (msg,4,"Connection close failed");}
            return (msg,0,"OK");   
        }

        public static (int,string) AddMessage(MessageData msg){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = @"INSERT INTO Message(chat_id,message_time,message_type,message_value,sender_user_id,message_status, sender_name, is_teacher)
                            VALUES (@CHID, @TIME,@TYPE,@VALUE,@SENDER,@STATUS,@NAME, @IST);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@CHID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@TIME",System.Data.SqlDbType.DateTime);
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@VALUE",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SENDER",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@STATUS",System.Data.SqlDbType.Int);
            command.Parameters["@CHID"].Value = msg.Chat_ID;
            command.Parameters["@TIME"].Value = msg.Time;
            command.Parameters["@TYPE"].Value = msg.Type;
            command.Parameters["@VALUE"].Value = msg.Value;
            command.Parameters["@SENDER"].Value = msg.Sender_ID;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            command.Parameters["@STATUS"].Value = msg.Status;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (int,string) UpdateChat(ChatData chat){
            (ChatData cd,int err,String s) = DataBaseManager.ReadChat(chat.ID);
            if(err == 3){
                return (err,s);
            }
            if(cd == null){
                return (1,"Chat does not exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "UPDATE Chat SET submission_id=@SID,chat_status= @STATUS,chat_type=@TYPE WHERE chat_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@STATUS",System.Data.SqlDbType.Int);
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = chat.ID;
            command.Parameters["@SID"].Value = chat.Submission_ID;
            command.Parameters["@STATUS"].Value = chat.Status;
            command.Parameters["@TYPE"].Value = chat.Type;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (int,string) UpdateMessage(MessageData msg,int id){
            (MessageData m,int err,String s) = DataBaseManager.ReadMessage(id);
            if(err == 3){
                return (err,s);
            }
            if(m == null){
                return (1,"Message does not exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "UPDATE Message SET chat_id=@CHID,message_time=@TIME,message_type=@TYPE,message_value=@VALUE,sender_user_id=@SENDER,message_status=@STATUS,course_id=@CID WHERE message_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.Int);
            command.Parameters.Add("@CHID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@TIME",System.Data.SqlDbType.DateTime);
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@VALUE",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SENDER",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@STATUS",System.Data.SqlDbType.Int);
            command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = id;
            command.Parameters["@CHID"].Value = msg.Chat_ID;
            command.Parameters["@TIME"].Value = msg.Time;
            command.Parameters["@TYPE"].Value = msg.Type;
            command.Parameters["@VALUE"].Value = msg.Value;
            command.Parameters["@SENDER"].Value = msg.Sender_ID;
            command.Parameters["@STATUS"].Value = msg.Status;
            command.Parameters["@CID"].Value = msg.Course_ID;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (List<int>,int,string) ReadMessagesIdOfChat(string chat_id){
            List<int> lst = new List<int>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT message_id FROM Message WHERE chat_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = chat_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add((int)dataReader.GetValue(0));
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }

        public static (List<(string,string)>,int,string) ReadExercisesIdAndNamesOfCourse(string course_id){
            List<(string,string)> lst = new List<(string,string)>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT exercise_id,exercise_name FROM Exercise WHERE course_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = course_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add((dataReader.GetValue(0).ToString(),dataReader.GetValue(1).ToString()));
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }

        public static (List<Message>,int,string) ReadMessagesOfChat(string chat_id){
            List<Message> lst = new List<Message>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Message WHERE chat_id=@ID ORDER BY message_time;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = chat_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()) {
                    var data = new Message {
                        ChatID = dataReader.GetValue(1).ToString(),
                        Date = (DateTime)dataReader.GetValue(2),
                        IsFile = (int) dataReader.GetValue(3) == 1,
                        Body = dataReader.GetValue(4).ToString(),
                        SenderID = dataReader.GetValue(5).ToString(),
                        Status = (int) dataReader.GetValue(6),
                        IsTeacher = (int) dataReader.GetValue(8) == 1,
                        SenderName = dataReader.GetValue(9).ToString()
                    };
                    lst.Add(data);
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }

        public static (bool,int,string) IsMetargelInCourse(string course_id,string metargel_id){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (false,3,"Connection failed");}
            String sql = "SELECT * FROM Metargel_Course WHERE course_id = @CID AND user_id = @UID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = metargel_id;
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

        public static (int,string) AddMetargelToCourse(string course_id,string metargel_id){
            (bool exist,int err,String s) = DataBaseManager.IsMetargelInCourse(course_id,metargel_id);
            if(exist){
                return (1,"The Metargel is already in the course");
            }
            if(err == 3){
                return (err,s);
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Metargel_Course VALUES (@UID, @CID);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = metargel_id;
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

        public static (List<string>,int,string) ReadCoursesIdOfMetargel(string metargel_id){
            List<string> lst = new List<string>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT course_id FROM Metargel_Course WHERE user_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = metargel_id;
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

        public static (List<string>,int,string) ReadMetargelsIdOfCourse(string course_id){
            List<string> lst = new List<string>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT user_id FROM Metargel_Course WHERE course_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
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

        public static (Dictionary<string,(string,int)>,int,string) ReadStudentsSubmissionsIdOfExercise(string exercise_id){
            Dictionary<string,(string,int)> dct = new Dictionary<string,(string,int)>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT user_id,submission_id,submitter_type FROM Submitters WHERE exercise_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = exercise_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    dct.Add(dataReader.GetValue(0).ToString(),(dataReader.GetValue(1).ToString(),(int)dataReader.GetValue(2)));
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (dct,4,"Connection close failed");}
            return (dct,0,"OK");   
        }

        public static (List<MessageData>,int,string) ReadNewMessagesOfCourse(string course_id){
            List<MessageData> lst = new List<MessageData>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Message WHERE course_id = @ID AND message_status = "+WAITING_TO_METARGEL+" ORDER BY message_time;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = course_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add(new MessageData(dataReader.GetValue(1).ToString(),(DateTime)dataReader.GetValue(2),(int)dataReader.GetValue(3),dataReader.GetValue(4).ToString(),dataReader.GetValue(5).ToString(),(int)dataReader.GetValue(6),dataReader.GetValue(7).ToString()));
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }

        public static (List<Test>,int,string) ReadTestsOfExercise(string exercise_id){
            List<Test> lst = new List<Test>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Test WHERE exercise_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = exercise_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add(new Test((int)dataReader.GetValue(0),dataReader.GetValue(1).ToString(),dataReader.GetValue(2).ToString(),dataReader.GetValue(3).ToString(),dataReader.GetValue(4).ToString(),(int)dataReader.GetValue(5),dataReader.GetValue(6).ToString(),dataReader.GetValue(7).ToString(),dataReader.GetValue(8).ToString(),(int)dataReader.GetValue(9)));
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");
        }

        public static (int,string) DeleteTestsOfExercise(string exercise_id){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "DELETE FROM Test WHERE exercise_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = exercise_id;
            try{
                command.ExecuteReader();
            } catch{
                try{cnn.Close();}catch{}
                return (2,"Delete failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (int,string) AddTest(Test test){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Test VALUES (@VALUE,@INPUT,@EOUTPUT,@OFNAME,@ARGS,@TIMEOUT,@MAIN,@ADD,@EID,@TYPE);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@EID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@VALUE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@INPUT",System.Data.SqlDbType.NText);
            command.Parameters.Add("@EOUTPUT",System.Data.SqlDbType.NText);
            command.Parameters.Add("@OFNAME",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@ARGS",System.Data.SqlDbType.NText);
            command.Parameters.Add("@TIMEOUT",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MAIN",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@ADD",System.Data.SqlDbType.NVarChar);
            command.Parameters["@TYPE"].Value = test.Type;
            command.Parameters["@EID"].Value = test.Exercise_ID;
            command.Parameters["@VALUE"].Value = test.Value;
            command.Parameters["@INPUT"].Value = test.Input;
            command.Parameters["@EOUTPUT"].Value = test.Expected_Output;
            command.Parameters["@OFNAME"].Value = test.Output_File_Name;
            command.Parameters["@ARGS"].Value = test.ArgumentsString;
            command.Parameters["@TIMEOUT"].Value = test.Timeout_In_Seconds;
            command.Parameters["@MAIN"].Value = test.Main_Sourse_File;
            command.Parameters["@ADD"].Value = test.AdittionalFilesLocation;
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
