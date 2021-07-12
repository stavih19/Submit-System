using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.Json;
using System.Web;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer;
using System.Text.RegularExpressions;
namespace Submit_System
{
    public enum ResourceType { Course, Exercise, Submission, Chat, Date, Test }
    public partial class DataBaseManager{
        public static readonly Dictionary<string, string> LongQueries;
        public static readonly Dictionary<(Role, ResourceType), string> PermQueries;
        public static readonly Dictionary<Role, string> RoleToCourseTable;
        private static Dictionary<string, string> GetQueries()
        {
            var file = File.ReadAllText(Path.Combine("Model", "Utils", "Queries.sql"));
            var dict = new Dictionary<string, string>();
            var queries = file.Split("---");
            for(int i = 1; i < queries.Length-1; i += 2)
            {
                dict.Add(queries[i], queries[i+1]);
            }
            return dict;
        }
        static DataBaseManager()
        {
            LongQueries = GetQueries();
            RoleToCourseTable = new Dictionary<Role, string>() {
            [Role.Student] = "Student_Course",
            [Role.Checker] = "Checker_Course",
            [Role.Teacher] = "Metargel_Course",
            };
            PermQueries = new Dictionary<(Role, ResourceType), string>
            {
                [(Role.Student, ResourceType.Course)] = "SELECT 1 FROM Student_Course WHERE course_id = @RID AND user_id = @UID;",
                [(Role.Teacher, ResourceType.Course)] = "SELECT 1 FROM Metargel_Course WHERE course_id = @RID AND user_id = @UID;",
                [(Role.Student, ResourceType.Exercise)] = LongQueries["HasExercise"].Replace("@Table", RoleToCourseTable[Role.Student]),
                [(Role.Teacher, ResourceType.Exercise)] = LongQueries["HasExercise"].Replace("@Table", RoleToCourseTable[Role.Teacher]),
                [(Role.Student, ResourceType.Submission)] = LongQueries["HasSubmissionStudent"],
                [(Role.Teacher, ResourceType.Submission)] = LongQueries["HasSubmissionTeacher"],
                [(Role.Student, ResourceType.Chat)] = LongQueries["HasChatStudent"],
                [(Role.Teacher, ResourceType.Chat)] = LongQueries["HasChatTeacher"],
                [(Role.Teacher, ResourceType.Date)] = LongQueries["HasDate"],
                [(Role.Teacher, ResourceType.Test)] = LongQueries["HasTest"],

            };
        }
        public static (List<Course>,int,string) GetCoursesOfUser(string student_id, Role role){
            List<Course> lst = new List<Course>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["Courses"].Replace("@Table", RoleToCourseTable[role]);
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",SqlDbType.VarChar);
            command.Parameters["@ID"].Value = student_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add( new Course {
                        ID = dataReader.GetValue(0).ToString(),
                        Name = dataReader.GetValue(1).ToString(),
                        Number = (int) dataReader.GetValue(2),
                        Year = (int) dataReader.GetValue(3),
                        Semester = (int) dataReader.GetValue(4)
                    });
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");} 
            return (lst, 0, "OK");
        }
        public static (List<ExerciseDateDisplay>, int, string) GetExerciseDates(string userid) {
            var lst = new List<ExerciseDateDisplay>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["StudentDates"];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",SqlDbType.VarChar);
            command.Parameters["@ID"].Value = userid;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    // C.course_id, C.course_name, C.course_number, E.exercise_id, E.exercise_name, D.submission_date
                    ExerciseDateDisplay display = new ExerciseDateDisplay {
                        CourseID = dataReader.GetValue(0).ToString(),
                        CourseName = dataReader.GetValue(1).ToString(),
                        CourseNumber = (int) dataReader.GetValue(2),
                        ID = dataReader.GetValue(3).ToString(),
                        Name = dataReader.GetValue(4).ToString(),
                        Date = (DateTime) dataReader.GetValue(5)
                    };
                    lst.Add(display);
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }
        /*
            C.course_id, C.course_name, C.course_number, E.exercise_id, E.exercise_name,
            E.style_test_grade_value, E.style_test_grade_value, E.late_submission_settings,
            S.auto_grade, S.style_grade, S.time_submitted 
        */
        public static (List<ExerciseGradeDisplay>, int, string) GetExerciseGrades(string userid) {
            var lst = new List<ExerciseGradeDisplay>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["StudentGrades"];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",SqlDbType.VarChar);
            command.Parameters["@ID"].Value = userid;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    /*
                        0 C.course_id,  1 C.course_name, 2 C.course_number, 3 E.exercise_id, 4 E.exercise_name,
                        5 E.auto_test_grade_value, 6 E.style_test_grade_value, 7 Date 8 TimeSubmitted 9 settings
                        10 S.auto_grade, 11 S.style_grade 12 manual_final_grade
                    */
                    var calc = new GradeCalculator {
                        AutoWeight = (int) dataReader.GetValue(5),
                        StyleWeight = (int) dataReader.GetValue(6),
                        Date = (DateTime) dataReader.GetValue(7),
                        TimeSubmitted = (DateTime) dataReader.GetValue(8),
                        LateSubmissionSettings = dataReader.GetValue(9).ToString(),
                        AutoGrade = (int) dataReader.GetValue(10),
                        StyleGrade = (int) dataReader.GetValue(11),
                        ManualGrade = (int) dataReader.GetValue(12)
                    };
                    ExerciseGradeDisplay display = new ExerciseGradeDisplay {
                        CourseID = dataReader.GetValue(0).ToString(),
                        CourseName = dataReader.GetValue(1).ToString(),
                        CourseNumber = (int) dataReader.GetValue(2),
                        ExID = dataReader.GetValue(3).ToString(),
                        ExName = dataReader.GetValue(4).ToString(),
                        Grade = calc.CalculateGrade()
                    };
                    lst.Add(display);
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }
        private class Desc : Comparer<string> {
            public override int Compare(string a , string b) {
                return b.CompareTo(a);
            }
        }
         public static (SortedDictionary<string, List<ExerciseLabel>>, int, string) GetAllExercises(string courseId) {
            var dict = new SortedDictionary<string, List<ExerciseLabel>>(new Desc());
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["AllExercises"];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@CID",SqlDbType.VarChar);
            command.Parameters["@CID"].Value = courseId;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    string year = dataReader.GetValue(0).ToString();
                     if(!dict.ContainsKey(year)) {
                        dict.Add(year, new List<ExerciseLabel>());
                    }
                    string id = dataReader.GetValue(1)?.ToString();
                    if(id != null) {
                        ExerciseLabel label = new ExerciseLabel(
                            id,
                            dataReader.GetValue(2)?.ToString()
                        );
                        dict[year].Add(label);
                    }
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (dict,4,"Connection close failed");}
            return (dict,0,"OK");   
        }
        public static (List<ExerciseGradeDisplay>, int, string) GetStudentSubmission(string userid) {
            var lst = new List<ExerciseGradeDisplay>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["StudentGrades"];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",SqlDbType.VarChar);
            command.Parameters["@ID"].Value = userid;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    /*
                        0 C.course_id,  1 C.course_name, 2 C.course_number, 3 E.exercise_id, 4 E.exercise_name,
                        5 E.auto_test_grade_value, 6 E.style_test_grade_value, 7 S.auto_grade, 8 S.style_grade,
                        9 manual_grade 10 reduction 
                    */
                    var settings = dataReader.GetValue(8).ToString().Split('_');
                    int days = Int32.Parse(settings[0]);
                    int penalty = Int32.Parse(settings[1]);
                    var calc = new GradeCalculator {
                        AutoWeight = (int) dataReader.GetValue(5),
                        StyleWeight = (int) dataReader.GetValue(6),
                        AutoGrade = (int) dataReader.GetValue(7),
                        StyleGrade = (int) dataReader.GetValue(8),
                        ManualGrade = (int) dataReader.GetValue(9),
                        InitReduction = (int) dataReader.GetValue(10),
                    };
                    ExerciseGradeDisplay display = new ExerciseGradeDisplay {
                        CourseID = dataReader.GetValue(0).ToString(),
                        CourseName = dataReader.GetValue(1).ToString(),
                        CourseNumber = (int) dataReader.GetValue(2),
                        ExID = dataReader.GetValue(3).ToString(),
                        ExName = dataReader.GetValue(4).ToString(),
                        Grade = calc.CalculateGrade()
                    };
                    lst.Add(display);
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }
        public static (List<ExerciseLabel>,int,string) ReadExerciseLabelsOfCourse(string course_id){
            List<ExerciseLabel> lst = new List<ExerciseLabel>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = "SELECT exercise_id,exercise_name FROM Exercise WHERE course_id = @ID ORDER BY exercise_id DESC;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",SqlDbType.VarChar);
            command.Parameters["@ID"].Value = course_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add(new ExerciseLabel(dataReader.GetValue(0).ToString(),dataReader.GetValue(1).ToString()));
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }
         public static (List<ExTeacherDisplay>,int,string) ReadTeacherExerciseLabels(string userid){
            var lst = new List<ExTeacherDisplay>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["TeacherExercises"];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",SqlDbType.VarChar);
            command.Parameters["@ID"].Value = userid;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add( new ExTeacherDisplay {
                        CourseName = (string) dataReader.GetValue(0),
                        CourseNumber = (int) dataReader.GetValue(1),
                        CourseID = (string) dataReader.GetValue(2),
                        ExID = (string) dataReader.GetValue(3),
                        ExName = (string) dataReader.GetValue(4),
                    });
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }

        public static (bool,int,string) CheckResourcePerms(string resource_id,string userID, Role role, ResourceType r){
            SqlConnection cnn  = new SqlConnection(connetionString);
            if(!PermQueries.ContainsKey((role, r)))
            {
                return (false, 0, "OK");
            }
            try{cnn.Open();} catch {return (false,3,"Connection failed");}
            String sql = PermQueries[(role, r)];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@UID",SqlDbType.VarChar);
            command.Parameters.Add("@RID",SqlDbType.VarChar);
            command.Parameters["@UID"].Value = userID;
            command.Parameters["@RID"].Value = resource_id;
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
        public static (List<UserLabel>,int,string) ReadUsersOfCourse(string course_id, Role role){
            List<UserLabel> lst = new List<UserLabel>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["UsersCourse"];
            sql = sql.Replace("@Table", RoleToCourseTable[role]);
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@CID",SqlDbType.VarChar);
            command.Parameters["@CID"].Value = course_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add( new UserLabel {
                        ID = dataReader.GetValue(0).ToString(),
                        Name = dataReader.GetValue(1).ToString(),
                    });
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }
         public static (List<UserLabel>,int,string) GetExerciseCheckers(string exercise_id){
            List<UserLabel> lst = new List<UserLabel>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            string sql = LongQueries["ExerciseCheckers"];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",SqlDbType.VarChar);
            command.Parameters["@ID"].Value = exercise_id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add( new UserLabel {
                        ID = dataReader.GetValue(0).ToString(),
                        Name = dataReader.GetValue(1).ToString(),
                    });
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }
        public static (Dictionary<string, List<UserLabel>>, int, string) GetSubmissionLabels(string courseId) {
            var dict = new Dictionary<string, List<UserLabel>>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["SubmissionLabels"];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",SqlDbType.VarChar);
            command.Parameters["@ID"].Value = courseId;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    string id = dataReader.GetValue(0).ToString();
                     if(!dict.ContainsKey(id)) {
                        dict.Add(id, new List<UserLabel>());
                    }
                    string userid = dataReader.GetValue(1)?.ToString();
                    if(id != null) {
                        UserLabel label = new UserLabel {
                            ID = userid,
                            Name = dataReader.GetValue(2).ToString()
                        };
                        dict[id].Add(label);
                    }
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (dict,4,"Connection close failed");}
            return (dict,0,"OK");   
        }
        public static (Dictionary<ChatType, Chat>,int,string) ReadChatsOfSubmission(string id) {
            var chats = new Dictionary<ChatType, Chat>() {
                [ChatType.Extension] = null,
                [ChatType.Appeal] = null
            };
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = @"SELECT chat_id, chat_type, chat_status
                            FROM Chat WHERE submission_id = @ID ORDER BY chat_type;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = id;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                   Chat chat = new Chat {
                        ID = dataReader.GetValue(0).ToString(),
                        Type = (ChatType) dataReader.GetValue(1),
                        State = (ChatState) dataReader.GetValue(2),
                        IsClosed = (int) dataReader.GetValue(2) == 1,
                    };
                    chats[chat.Type] = chat;
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (chats,4,"Connection close failed");}
            return (chats,0,"OK");   
        }
        public static (List<UserLabel>,int,string) GetSubmitters(string submssionId) {
            var submitters = new List<UserLabel>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = @"SELECT U.user_id, U.name FROM Submitters as SS INNER JOIN Users AS U
                        ON SS.user_id = U.user_id WHERE SS.submission_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = submssionId;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    submitters.Add( new UserLabel {
                        ID = dataReader.GetValue(0).ToString(),
                        Name = dataReader.GetValue(1).ToString(),
                    });
                }
            } catch {
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (submitters,4,"Connection close failed");}
            return (submitters,0,"OK");   
        }

        public static ((string,int),int,string) ReadSubmissionAndTypeOfStudentInExercise(string student_id,string exercise_id){
            string submission_id;
            int type;
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return ((null,-1),3,"Connection failed");}
            String sql = @"SELECT submission_id,submitter_type FROM Submitters WHERE user_id = @UID AND exercise_id = @EID AND submitter_type != 3;";
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
        public static (int,string) AddUserToCourse(string course_id,string user_id, Role role){
            (bool exist,int err,String s) = DataBaseManager.CheckResourcePerms(course_id,user_id, role, ResourceType.Course);
            if(exist){
                return (1,"The User is already in the course");
            }
            if(err == 3){
                return (err,s);
            }
            (User u, int code, string msg) = DataBaseManager.ReadUser(user_id);
            if(u == null) {
                return (code, msg);
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO @Table VALUES (@UID, @CID);";
            sql = sql.Replace("@Table", RoleToCourseTable[role]);
            SqlCommand command = new SqlCommand(sql,cnn);

            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = user_id;
            command.Parameters["@CID"].Value = course_id;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (3,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static (int,string) AddCheckerToExercise(string course_id,string user_id){
            (bool exist,int err,String s) = DataBaseManager.CheckResourcePerms(course_id,user_id, Role.Checker, ResourceType.Course);
            if(exist){
                return (2,"Checker not in course");
            }
            if(err == 3){
                return (err,s);
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Checker_Exercise VALUES (@UID, @EID);";
            SqlCommand command = new SqlCommand(sql,cnn);

            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@EID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = user_id;
            command.Parameters["@EID"].Value = course_id;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (3,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
         public static (int,string) DeleteCheckerFromExercise(string exercise_id,string user_id){
            (User u, int code, string msg) = DataBaseManager.ReadUser(user_id);
            if(u == null) {
                return (code, msg);
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "DELETE FROM Checekr_Exercise WHERE user_id = @UID AND exercise_id = @EID;";
            SqlCommand command = new SqlCommand(sql,cnn);

            command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@EID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@UID"].Value = user_id;
            command.Parameters["@EID"].Value = exercise_id;
            try{
                command.ExecuteReader();
            } catch {
                try{cnn.Close();}catch{}
                return (3,"Delete failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static (int,string) DeleteUserFromCourse(string course_id,string user_id, Role role){
            (bool exist,int err,String s) = DataBaseManager.CheckResourcePerms(course_id,user_id, role, ResourceType.Course);
            if(!exist && err == 0){
                return (2,$"User is not a {role.ToString()} in this course");
            }
            else if(err == 3)
            {
                return (err, s);
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            if(role == Role.Checker)
            {
                string checkerQuery = LongQueries["DeleteChecker"];
                using (SqlCommand command = new SqlCommand(checkerQuery,cnn))
                {
                    command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
                    command.Parameters["@UID"].Value = user_id;
                    command.Parameters["@CID"].Value = course_id;
                    command.ExecuteNonQuery();
                }
            }
            String sql = "DELETE FROM @Table WHERE user_id = @UID AND course_id = @CID;";
            sql = sql.Replace("@Table", RoleToCourseTable[role]);
            using (SqlCommand command = new SqlCommand(sql,cnn))
            {
                command.Parameters.Add("@UID",System.Data.SqlDbType.NVarChar);
                command.Parameters.Add("@CID",System.Data.SqlDbType.NVarChar);
                command.Parameters["@UID"].Value = user_id;
                command.Parameters["@CID"].Value = course_id;
                command.ExecuteNonQuery();
            } 
            try{
                
            } catch {
                try{cnn.Close();}catch{}
                return (3,"delete failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static (List<SubmitDate>,int,string) GetStudentExeriseDates(int dateID, string exerciseID){
            var dates = new List<SubmitDate>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = @"SELECT submission_date, reduction, [group] FROM Submission_Dates
                        WHERE submission_date_id = @DID OR (exercise_id = @EID AND [group] = 0)
                        ORDER BY submission_date, reduction;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@DID",System.Data.SqlDbType.Int);
            command.Parameters["@DID"].Value = dateID;
            command.Parameters.Add("@EID",System.Data.SqlDbType.VarChar);
            command.Parameters["@EID"].Value = exerciseID;
            // Only retrieve dates after the latest date with no reduction
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                int firstRelevantDateIndex = 0;
                while(dataReader.Read()){
                    int reduction = (int) dataReader.GetValue(1);
                    if(reduction == 0) 
                    {
                        firstRelevantDateIndex = dates.Count;
                    }
                    dates.Add( new SubmitDate {
                            Date = (DateTime) dataReader.GetValue(0),
                            Reduction = reduction,
                            Group = (int) dataReader.GetValue(2)
                        });
                }
                dates.RemoveRange(0, firstRelevantDateIndex);
            } catch{
                try{cnn.Close();}catch{}
                return (null,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (dates,4,"Connection close failed");}
            return (dates,0,"OK");   
        }
        public static (int,string) CloseChat(string chatid){
            (ChatData cd,int err,String s) = DataBaseManager.ReadChat(chatid);
            if(err == 3){
                return (err,s);
            }
            if(cd == null){
                return (2,"Chat does not exist");
            }
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "UPDATE Chat SET chat_status=1 WHERE chat_id=@ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.VarChar);
            command.Parameters["@ID"].Value = chatid;
            try{
                command.ExecuteNonQuery();
            } catch {
                try{cnn.Close();}catch{}
                return (3,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static (int,string) UpdatePassword(string userid, string hash){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}

            string sql = "UPDATE Users SET password_hash=@HASH WHERE user_id = @ID;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@HASH",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = userid;
            command.Parameters["@HASH"].Value = hash;
            try{
                command.ExecuteNonQuery();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static (int,string) AddPasswordToken(string userid, string passwordToken){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "INSERT INTO Tokens VALUES (@ID, @TOKEN, DATEADD(day, , GETDATE()))";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@TOKEN",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = userid;
            command.Parameters["@TOKEN"].Value = passwordToken;
            try{
                command.ExecuteNonQuery();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static (int,string) DeleteToken(string userid) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "DELETE FROM Tokens WHERE user_id=@ID";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters["@ID"].Value = userid;
            try{
                command.ExecuteNonQuery();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Update failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static ((string, DateTime),int,string) ReadToken(string passwordToken) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            string id = null;
            DateTime date = DateTime.MinValue;
            try{cnn.Open();} catch {return ((id, date),3,"Connection failed");}
            String sql = "SELECT user_id, creation_date FROM Users WHERE token = @TOKEN;";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@TOKEN",System.Data.SqlDbType.NVarChar);
            command.Parameters["@TOKEN"].Value = passwordToken;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                if(!dataReader.Read()){
                    try{cnn.Close();}catch{}
                    return ((id, date),2,"token does not exist");
                }
                id = (string) dataReader.GetValue(0);
                date = (DateTime) dataReader.GetValue(1);
            } catch {
                try{cnn.Close();}catch{}
                return ((id, date),3,"Connection failed");
            }
            try{cnn.Close();} catch{return ((id, date),4,"Connection close failed");}
            return ((id, date),0,"OK");   
        }
        // C.chat_id, M.sender_user_id, M.sender_name, M.message_value
        public static (List<RequestLabel>,int,string) GetRequests(string exerciseID, ChatType type) {
            SqlConnection cnn  = new SqlConnection(connetionString);
            var lst = new List<RequestLabel>();
            DateTime date = DateTime.MinValue;
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = LongQueries["GetRequests"];
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@EID",System.Data.SqlDbType.VarChar);
            command.Parameters["@EID"].Value = exerciseID;
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters["@TYPE"].Value = (int) type;
            try{
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read()){
                    lst.Add( new RequestLabel {
                        ChatID = dataReader.GetValue(0).ToString(),
                        StudentID = dataReader.GetValue(1).ToString(),
                        StudentName = dataReader.GetValue(2).ToString(),
                        Message = dataReader.GetValue(3).ToString(),
                        Type = type
                    });
                }
            } catch {
                try{cnn.Close();}catch{}
                return (lst,3,"Connection failed");
            }
            try{cnn.Close();} catch{return (lst,4,"Connection close failed");}
            return (lst,0,"OK");   
        }
    }
    

}