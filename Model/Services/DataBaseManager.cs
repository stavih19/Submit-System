using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;

namespace Submit_System
{
    public partial class DataBaseManager{
        private static string connetionString = MyConfig.Configuration.GetSection("ConnectionString").Value;

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
                u = new User(dataReader.GetValue(0).ToString(),
                dataReader.GetValue(1)?.ToString(),
                dataReader.GetValue(2)?.ToString(),
                dataReader.GetValue(3)?.ToString()
                );
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
            command.Parameters["@PWD"].Value = OrNull(user.PasswordHash);
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
                c = new Course(
                    dataReader.GetValue(0).ToString(),
                    (int) dataReader.GetValue(1),
                    dataReader.GetValue(2).ToString(),
                    (int)dataReader.GetValue(3),
                    (int)dataReader.GetValue(4)
                );
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
                ex.CourseID = dataReader.GetValue(2).ToString();
                ex.OriginalExerciseID = dataReader.GetValue(3).ToString();
                ex.MaxSubmitters = (int)dataReader.GetValue(4);
                ex.FilesLocation = dataReader.GetValue(5).ToString();
                ex.LateSubmissionSettings = dataReader.GetValue(6).ToString();
                ex.ProgrammingLanguage = dataReader.GetValue(7).ToString();
                ex.AutoTestGradeWeight = (int)dataReader.GetValue(8);
                ex.StyleTestGradeWeight = (int)dataReader.GetValue(9);
                ex.IsActive = (int)dataReader.GetValue(10);
                ex.MultipleSubmission = (int)dataReader.GetValue(11) == 1;
                ex.MossShownMatches = (int) dataReader.GetValue(12);
                ex.MossMaxTimesMatch = (int) dataReader.GetValue(13);
                ex.MossLink = dataReader.GetValue(14)?.ToString();
                ex.Filenames = FileUtils.GetRelativePaths(Path.Combine(ex.FilesLocation, "Help"));
                
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
            String sql = "INSERT INTO Exercise VALUES (@ID,@NAME,@CID,@OEID,@MAX,@FILES,@LATEST,@LANG,@AUTO,@STYLE,@ACTIVE,@MULT, @SHOW, @MAXM, null);";
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
            command.Parameters.Add("@SHOW",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MAXM",System.Data.SqlDbType.Int);
            command.Parameters["@ID"].Value = exercise.ID;
            command.Parameters["@NAME"].Value = exercise.Name;
            command.Parameters["@CID"].Value = exercise.CourseID;
            command.Parameters["@OEID"].Value = OrNull(exercise.OriginalExerciseID);
            command.Parameters["@MAX"].Value = exercise.MaxSubmitters;
            command.Parameters["@FILES"].Value = exercise.FilesLocation;
            command.Parameters["@LATEST"].Value = exercise.LateSubmissionSettings;
            command.Parameters["@LANG"].Value = exercise.ProgrammingLanguage;
            command.Parameters["@AUTO"].Value = exercise.AutoTestGradeWeight;
            command.Parameters["@STYLE"].Value = exercise.StyleTestGradeWeight;
            command.Parameters["@ACTIVE"].Value = exercise.IsActive;
            command.Parameters["@MULT"].Value = exercise.MultipleSubmission ? 1 : 0;
            command.Parameters["@SHOW"].Value = exercise.MossShownMatches;
            command.Parameters["@MAXM"].Value = exercise.MossMaxTimesMatch;
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
            Submission sub = new Submission();
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
                sub.ExerciseID = dataReader.GetValue(1).ToString();
                sub.FilesLocation = dataReader.GetValue(2).ToString();
                sub.AutoGrade = (int)dataReader.GetValue(3);
                sub.StyleGrade = (int)dataReader.GetValue(4);
                sub.ManualFinalGrade = (int)dataReader.GetValue(5);
                sub.ManualCheckData = dataReader.GetValue(6).ToString();
                sub.SubmissionStatus = (int)dataReader.GetValue(7);
                sub.SubmissionDateId = (int)dataReader.GetValue(8);
                sub.TimeSubmitted = (DateTime)dataReader.GetValue(9);
                sub.HasCopied = (int)dataReader.GetValue(10) == 1;
                sub.CurrentCheckerId = dataReader.GetValue(11).ToString();
                sub.Filenames = FileUtils.GetRelativePaths(sub.FilesLocation);
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (sub,4,"Connection close failed");}
            return (sub,0,"OK");   
        }
        public static Object OrNull(Object obj)
        {
            return obj ?? DBNull.Value;
        }
        public static (int,string) AddSubmission(Submission submission){
            (Submission sub,int err,String s) = DataBaseManager.ReadSubmission(submission.ID);
            if(sub != null){
                return (1,"Submission is already exists");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Submission VALUES (@ID,@EID,@FL,@AG,@SG,@MFG,@MD,@STATUS,@DID,@SUBMITTED, 0, null);";
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
            command.Parameters["@ID"].Value = submission.ID;
            command.Parameters["@EID"].Value = submission.ExerciseID;
            command.Parameters["@FL"].Value = OrNull(submission.FilesLocation);
            command.Parameters["@AG"].Value = submission.AutoGrade;
            command.Parameters["@SG"].Value = submission.StyleGrade;
            command.Parameters["@MFG"].Value = submission.ManualFinalGrade;
            command.Parameters["@MD"].Value = OrNull(submission.ManualCheckData);
            command.Parameters["@STATUS"].Value = submission.SubmissionStatus;
            command.Parameters["@DID"].Value = submission.SubmissionDateId;
            command.Parameters["@SUBMITTED"].Value = submission.TimeSubmitted;
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
            String sql = @"UPDATE Exercise SET course_id=@CID,original_exercise_id=@OEID,
                        max_submitters=@MAX,test_files_location=@FILES,late_submission_settings=@LATEST,programming_language=@LANG,
                        auto_test_grade_value=@AUTO,style_test_grade_value=@STYLE,is_active=@ACTIVE,multiple_submissions=@MULT
                        WHERE exercise_id = @ID;";
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
            command.Parameters["@CID"].Value = exercise.CourseID;
            command.Parameters["@OEID"].Value = exercise.OriginalExerciseID;
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
    
        public static (int,string) DeleteStudentFromSubmission(string student_id,string submission_id){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "DELETE FROM Submitters WHERE user_id = @UID AND submission_id= @SID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = student_id;
            command.Parameters["@SID"].Value = submission_id;
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
            String sql = @"UPDATE Submission SET exercise_id=@EID,files_location=@FL,auto_grade=@AG,style_grade=@SG,
                        manual_final_grade=@MFG,manual_check_data=@MD,submission_status=@STATUS,submission_date_id=@DID,
                        time_submitted=@SUBMITTED, has_copied=@HC, current_checker_id=@CURRENT WHERE submission_id = @ID;";
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
            command.Parameters.Add("@HC",System.Data.SqlDbType.Int);
            command.Parameters.Add("@CURRENT",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = submission.ID;
            command.Parameters["@EID"].Value = submission.ExerciseID;
            command.Parameters["@FL"].Value = submission.FilesLocation;
            command.Parameters["@AG"].Value = submission.AutoGrade;
            command.Parameters["@SG"].Value = submission.StyleGrade;
            command.Parameters["@MFG"].Value = submission.ManualFinalGrade;
            command.Parameters["@MD"].Value = submission.ManualCheckData;
            command.Parameters["@STATUS"].Value = submission.SubmissionStatus;
            command.Parameters["@DID"].Value = submission.SubmissionDateId;
            command.Parameters["@SUBMITTED"].Value = submission.TimeSubmitted;
            command.Parameters["@HC"].Value = submission.HasCopied ? 1 : 0;
            command.Parameters["@CURRENT"].Value = submission.CurrentCheckerId;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }

        public static (Chat,int,string) ReadChat(string id) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT * FROM Chat WHERE chat_id = @ID;";
            Chat  ch;
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return (null,1,"Chat does not exist");
                }
                ch = new Chat {
                    ID = dataReader.GetValue(0).ToString(),
                    SubmissionID = dataReader.GetValue(1).ToString(),
                    State = (ChatState) dataReader.GetValue(2),
                    Type = (ChatType) dataReader.GetValue(3)
                };
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (ch,4,"Connection close failed");}
            return (ch,0,"OK");   
        }

        public static (int,string) AddChat(Chat chat){
            (Chat c,int err,String s) = DataBaseManager.ReadChat(chat.ID);
            if(err == 3){
                return (err,s);
            }
            if(c != null){
                return (5,"Chat is already exists");
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
            command.Parameters["@SID"].Value = chat.SubmissionID;
            command.Parameters["@STATUS"].Value = (int) chat.State;
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
                msg = new MessageData(dataReader.GetValue(1).ToString(),
                (DateTime)dataReader.GetValue(2),
                (int)dataReader.GetValue(3),
                dataReader.GetValue(4).ToString(),
                dataReader.GetValue(5).ToString(),
                (int)dataReader.GetValue(6),
                dataReader.GetValue(7).ToString());
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (msg,4,"Connection close failed");}
            return (msg,0,"OK");   
        }

        public static (int, int,string) AddMessage(Message msg){
            SqlConnection cnn  = new SqlConnection(connetionString);
            int id = -1;
            string name = ReadUser(msg.SenderID).Item1.Name;
            try{cnn.Open();} catch {return (id, 3,"Connection failed");}
            String sql = @"INSERT INTO Message(chat_id,attached_file,message_text,sender_user_id,message_status, sender_name, from_teacher)
                           OUTPUT Inserted.message_id
                           VALUES (@CHID,@FILE,@VALUE,@SENDER,@STATUS,@NAME, @IST);";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@CHID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@FILE",System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@VALUE",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@SENDER",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@STATUS",System.Data.SqlDbType.Int);
            command.Parameters.Add("@NAME",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@IST",System.Data.SqlDbType.Int);
            command.Parameters["@CHID"].Value = msg.ChatID;
            command.Parameters["@FILE"].Value = OrNull(msg.AttachedFilePath);
            command.Parameters["@VALUE"].Value = msg.Body;
            command.Parameters["@SENDER"].Value = msg.SenderID;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            command.Parameters["@STATUS"].Value = msg.Status;
            command.Parameters["@NAME"].Value = name;
            command.Parameters["@IST"].Value = msg.IsTeacher ? 1 : 0;
            try{
                id = (int) command.ExecuteScalar();
            } catch {
                try{cnn.Close();}catch{}
                return (id, 2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (id, 4,"Connection close failed");}
            return (id, 0,"OK");
        }

        public static (int,string) UpdateChat(Chat chat){
            (Chat cd,int err,String s) = DataBaseManager.ReadChat(chat.ID);
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
            command.Parameters["@SID"].Value = chat.SubmissionID;
            command.Parameters["@STATUS"].Value = (int) chat.State;
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
            String sql = @"UPDATE Message SET chat_id=@CHID,message_time=@TIME,message_type=@TYPE,message_value=@VALUE,
                        sender_user_id=@SENDER,message_status=@STATUS,course_id=@CID WHERE message_id = @ID;";
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
                // message_id int IDENTITY(1,1) ,
                // chat_id nvarchar(60),
                // message_time DATETIME DEFAULT GETDATE(),
                // attached_file varchar(128),
                // message_text ntext,
                // sender_user_id nvarchar(10),
                // message_status int,
                // course_id nvarchar(16),
                // from_teacher int,
                // sender_name nvarchar(64),
                while(dataReader.Read()) {
                    var data = new Message {
                        ID = (int) dataReader.GetValue(0),
                        ChatID = dataReader.GetValue(1).ToString(),
                        Date = (DateTime)dataReader.GetValue(2),
                        AttachedFilePath = dataReader.GetValue(3)?.ToString(),
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
                    lst.Add(new MessageData(dataReader.GetValue(1).ToString(),
                        (DateTime)dataReader.GetValue(2),
                        (int)dataReader.GetValue(3),
                        dataReader.GetValue(4).ToString(),
                        dataReader.GetValue(5).ToString(),
                        (int)dataReader.GetValue(6),
                        dataReader.GetValue(7).ToString())
                        );
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
                    lst.Add(new Test (
                        id : (int)dataReader.GetValue(0),
                        value : (int)dataReader.GetValue(1),
                        input : dataReader.GetValue(2).ToString(),
                        expected_output : dataReader.GetValue(3).ToString(),
                        output_file_name : dataReader.GetValue(4).ToString(),
                        arguments_string : dataReader.GetValue(5).ToString(),
                        timeout_in_seconds : (int)dataReader.GetValue(6),
                        main_sourse_file : dataReader.GetValue(7).ToString(),
                        adittional_files_location : dataReader.GetValue(8).ToString(),
                        exercise_id : dataReader.GetValue(9).ToString(),
                        type : (int)dataReader.GetValue(10))
                    );
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
        public static (int, int,string) AddTest(Test test){
            SqlConnection cnn  = new SqlConnection(connetionString);
            int id = -1;
            try{cnn.Open();} catch {return (id, 3,"Connection failed");}
            String sql = @"INSERT INTO Test([weight], input, expected_output, output_file_name,
                            arguments_string, timeout_in_seconds, main_source_file, additional_files_location, exercise_id, type)
                            OUTPUT Inserted.test_id
                            VALUES (@VALUE,@INPUT,@EOUTPUT,@OFNAME,@ARGS,@TIMEOUT,@MAIN,@ADD,@EID,@TYPE);";
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
            command.Parameters["@ARGS"].Value = test.Arguments_String;
            command.Parameters["@TIMEOUT"].Value = test.Timeout_In_Seconds;
            command.Parameters["@MAIN"].Value = test.Main_Sourse_File;
            command.Parameters["@ADD"].Value = test.Adittional_Files_Location;
            try{
                var reader = command.ExecuteReader();
                if(!reader.Read()) {
                    throw new Exception();
                }
                id = (int) reader.GetValue(0);
            } catch {
                try{cnn.Close();}catch{}
                return (id, 2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (id, 4,"Connection close failed");}
            return (id, 0,"OK");
        }

    }  
}
