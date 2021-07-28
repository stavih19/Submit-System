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
    public enum ResourceType { Course, Exercise, Submission, Chat, Date, Test, Message, OldExercise, OldTest }
    public partial class DataBaseManager{
        public static readonly Dictionary<string, string> LongQueries;
        public static readonly Dictionary<(Role, ResourceType), string> PermQueries;
        public static readonly Dictionary<Role, string> RoleToCourseTable;
        private static Dictionary<string, string> GetQueries()
        {
            string file = File.ReadAllText(Path.Combine("Model", "Utils", "Queries.sql"));
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
                [(Role.Checker, ResourceType.Course)] = "SELECT 1 FROM Checker_Course WHERE course_id = @RID AND user_id = @UID;",
                [(Role.Teacher, ResourceType.Course)] = "SELECT 1 FROM Metargel_Course WHERE course_id = @RID AND user_id = @UID;",
                [(Role.Student, ResourceType.Exercise)] = LongQueries["HasExercise"].Replace("@Table", RoleToCourseTable[Role.Student]),
                [(Role.Teacher, ResourceType.Exercise)] = LongQueries["HasExercise"].Replace("@Table", RoleToCourseTable[Role.Teacher]),
                [(Role.Student, ResourceType.Submission)] = LongQueries["HasSubmissionStudent"],
                [(Role.Teacher, ResourceType.Submission)] = LongQueries["HasSubmissionTeacher"],
                [(Role.Student, ResourceType.Chat)] = LongQueries["HasChatStudent"],
                [(Role.Teacher, ResourceType.Chat)] = LongQueries["HasChatTeacher"],
                [(Role.Teacher, ResourceType.Date)] = LongQueries["HasDate"],
                [(Role.Teacher, ResourceType.Test)] = LongQueries["HasTest"],
                [(Role.Student, ResourceType.Message)] = LongQueries["HasMessage"].Replace("@Table", RoleToCourseTable[Role.Student]),
                [(Role.Teacher, ResourceType.Message)] = LongQueries["HasMessage"].Replace("@Table", RoleToCourseTable[Role.Teacher]),
                [(Role.Teacher, ResourceType.OldExercise)] = LongQueries["HasOldExercise"],
                [(Role.Teacher, ResourceType.OldTest)] = LongQueries["HasOldTest"]
            };
        }
        public static SqlParameter CreateParameter<T>(string name, SqlDbType dbType, T value)
        {
            var param = new SqlParameter(name, dbType);
            param.Value = (Object) value ?? DBNull.Value;
            return param;
        }
        public static (T,DBCode) SingleRecordQuery<T>(string sql, SqlParameter[] parameters, Func<SqlDataReader, T> convert)
        {
            List<T> lst;
            try
            {
                lst = Query(sql, parameters, convert);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.ToString());
                return (default(T), DBCode.Error);
            }
            if(lst.Count == 0)
            {
                return (default(T), DBCode.NotFound);
            }
            return (lst[0], DBCode.OK);
        }
        public static DBCode HandleNonQuery(string sql, SqlParameter[] parameters)
        {
            int rows;
            try
            {
                rows = NonQuery(sql, parameters);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.ToString());
                return DBCode.Error;
            }
            if(rows == 0)
            {
                return DBCode.NotFound;
            }
            return DBCode.OK;
        }
        public static void SetNull(SqlParameter[] parameters)
        {
            foreach(var param in parameters)
            {
                if(param.Value == null)
                {
                    param.Value = DBNull.Value;
                }
            }
        }
        public static int NonQuery(string sql, SqlParameter[] parameters) {
            SetNull(parameters);
            using(SqlConnection cnn  = new SqlConnection(connetionString))
            {
                cnn.Open();
                using (SqlCommand command = new SqlCommand(sql,cnn))
                {
                    command.Parameters.AddRange(parameters);
                    return command.ExecuteNonQuery();
                }
            }
        } 
        public static (List<T>, DBCode) MultiRecordQuery<T>(string sql, SqlParameter[] parameters, Func<SqlDataReader, T> convert)
        {
            List<T> lst;
            try 
            {
                lst = Query(sql, parameters, convert);
            }
            catch
            {
                return (null, DBCode.Error);
            }
            return (lst, DBCode.OK);
        }

        public static List<T> Query<T>(string sql, SqlParameter[] parameters, Func<SqlDataReader, T> convert) {
            SetNull(parameters);
            var lst = new List<T>();
            using(SqlConnection cnn  = new SqlConnection(connetionString))
            {
                cnn.Open();
                using (SqlCommand command = new SqlCommand(sql,cnn))
                {
                    command.Parameters.AddRange(parameters);
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            lst.Add(convert(reader));
                        }
                    }
                }
            }
            return lst;
        } 
        /// <summary>
        /// Overload of Query that takes a converter function without a return type. Data can be extracted with closures.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="convert"></param>
        public static void Query(string sql, SqlParameter[] parameters, Action<SqlDataReader> convert) {
            SetNull(parameters);
            using(SqlConnection cnn  = new SqlConnection(connetionString))
            {
                cnn.Open();
                using (SqlCommand command = new SqlCommand(sql,cnn))
                {
                    command.Parameters.AddRange(parameters);
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            convert(reader);
                        }
                    }
                }
            }
        }  
        public static (List<Course>,DBCode) GetCoursesOfUser(string student_id, Role role){
            String sql = LongQueries["Courses"].Replace("@Table", RoleToCourseTable[role]);
            SqlParameter[] parameters = { CreateParameter("@ID", SqlDbType.VarChar, student_id) };
            return MultiRecordQuery(sql, parameters, (SqlDataReader dataReader) =>
                new Course {
                    ID = dataReader.GetValue(0).ToString(),
                    Name = dataReader.GetValue(1).ToString(),
                    Number = (int) dataReader.GetValue(2),
                    Year = (int) dataReader.GetValue(3),
                    Semester = (int) dataReader.GetValue(4)
                }
            );
        }
        public static (List<ExerciseDateDisplay>, int, string) GetStudentExerciseDates(string userid) {
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
                    string settings = dataReader.GetValue(9).ToString();
                    var calc = new GradeCalculator {
                        AutoWeight = (int) dataReader.GetValue(5),
                        StyleWeight = (int) dataReader.GetValue(6),
                        Date = (DateTime) dataReader.GetValue(7),
                        TimeSubmitted = (DateTime) dataReader.GetValue(8),
                        Reductions = Exercise.ParseLateSettings(settings),
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

        public static DBCode CheckResourcePerms(string resourceId,string userID, Role role, ResourceType r){
            if(!PermQueries.ContainsKey((role, r)))
            {
                return DBCode.NotFound;
            }
            string sql = PermQueries[(role, r)];
            SqlParameter[] parameters = {
                CreateParameter("@UID",SqlDbType.VarChar, userID),
                CreateParameter("@RID",SqlDbType.VarChar, resourceId)
            };
            return SingleRecordQuery(sql, parameters, (SqlDataReader reader) => reader.GetValue(0)).Item2;
        }
        public static DBCode CheckResourcePerms(int resourceID,string userID, Role role, ResourceType r){
            if(!PermQueries.ContainsKey((role, r)))
            {
                return DBCode.NotFound;
            }
            string sql = PermQueries[(role, r)];
            SqlParameter[] parameters = {
                CreateParameter("@UID",SqlDbType.VarChar, userID),
                CreateParameter("@RID",SqlDbType.Int, resourceID)
            };
            return SingleRecordQuery(sql, parameters, (SqlDataReader reader) => reader.GetValue(0)).Item2;
        }
        public static (List<UserLabel>,DBCode) GetUsersOfCourse(string courseId, Role role){
            String sql = LongQueries["UsersCourse"];
            sql = sql.Replace("@Table", RoleToCourseTable[role]);
            SqlParameter[] parameters = {
                CreateParameter("@CID",System.Data.SqlDbType.VarChar, courseId)
            };
            return MultiRecordQuery(sql, parameters, (SqlDataReader dataReader) =>
                new UserLabel {
                    ID = dataReader.GetValue(0).ToString(),
                    Name = dataReader.GetValue(1).ToString(),
                }
            );
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
        public static (Dictionary<string, List<SubmissionLabel>>, int, string) GetSubmissionLabels(string exerciseId) {
            var dict = new Dictionary<string, SubmissionLabel>();
            String sql = LongQueries["SubmissionLabels"];
            SqlParameter[] parameters = {
                CreateParameter("@ID",SqlDbType.VarChar, exerciseId)
            };
            Query(sql, parameters, (SqlDataReader dataReader) => {
                    string id = dataReader.GetValue(0).ToString();
                     if(!dict.ContainsKey(id)) {
                        dict.Add(id, new SubmissionLabel());
                    }
                    dict[id].ID = id;
                    dict[id].State = (SubmissionState) dataReader.GetValue(3);
                    dict[id].CurrentChecker = dataReader.GetValue(4)?.ToString();
                    string userid = dataReader.GetValue(1)?.ToString();
                    if(id != null) {
                        UserLabel user = new UserLabel {
                            ID = userid,
                            Name = dataReader.GetValue(2).ToString()
                        };
                        dict[id].Submitters.Add(user);
                    }
            });
            var subLabels = new Dictionary<string, List<SubmissionLabel>>();
            foreach(var keyVal in dict)
            {
                string stateString = keyVal.Value.State.ToString();
                if(!subLabels.ContainsKey(stateString))
                {
                    subLabels[stateString] = new List<SubmissionLabel>();
                }
                subLabels[stateString].Add(keyVal.Value);
            }
            return (subLabels,0,"OK");   
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
        public static DBCode AddUserToCourse(string courseId,string userId, Role role){
            String sql = "IF NOT EXISTS(SELECT 1 FROM @Table WHERE course_id=@CID AND user_id=@UID) INSERT INTO @Table VALUES (@UID, @CID);";
            sql = sql.Replace("@Table", RoleToCourseTable[role]);

             SqlParameter[] parameters = {
                CreateParameter("@UID",System.Data.SqlDbType.NVarChar, userId),
                CreateParameter("@CID",System.Data.SqlDbType.NVarChar, courseId)
            };
            bool success;
            try
            {
                success = NonQuery(sql, parameters) == 1;
            }
            catch
            {
                return DBCode.Error;
            }
            if(success)
            {
                return DBCode.OK;
            }
            else
            {
                return DBCode.AlreadyExists;
            }
        }
        public static DBCode AddCheckerToExercise(string exerciseId,string userId){
            DBCode code = DataBaseManager.CheckResourcePerms(exerciseId,userId, Role.Checker, ResourceType.Course);
            if(code != DBCode.OK){
                return code;
            }
            string sql = "IF NOT EXISTS(SELECT 1 FROM Checker_Exercise WHERE exercise_id=@EID AND user_id=@UID) INSERT INTO Checker_Exercise VALUES (@UID, @EID);";
            SqlParameter[] parameters = {
                CreateParameter("@UID",System.Data.SqlDbType.NVarChar, userId),
                CreateParameter("@EID",System.Data.SqlDbType.NVarChar, exerciseId)
            };
            bool success;
            try
            {
                success = NonQuery(sql, parameters) == 1;
            }
            catch
            {
                return DBCode.Error;
            }
            if(success)
            {
                return DBCode.OK;
            }
            else
            {
                return DBCode.AlreadyExists;
            }
        }
         public static DBCode DeleteCheckerFromExercise(string exerciseId,string userId){
            string sql = "DELETE FROM Checker_Exercise WHERE user_id = @UID AND exercise_id = @EID;";
            SqlParameter[] parameters = {
                CreateParameter("@UID",System.Data.SqlDbType.NVarChar, userId),
                CreateParameter("@EID",System.Data.SqlDbType.NVarChar, exerciseId)
            };
            return HandleNonQuery(sql, parameters);
        }
        public static DBCode DeleteUserFromCourse(string courseId,string userId, Role role){
             SqlParameter[] parameters = {
                CreateParameter("@UID",System.Data.SqlDbType.NVarChar, userId),
                CreateParameter("@CID",System.Data.SqlDbType.NVarChar, courseId)
            };
            if(role == Role.Checker)
            {
                string checkerQuery = LongQueries["DeleteChecker"];
                DBCode code = HandleNonQuery(checkerQuery, parameters);
                if(code == DBCode.Error)
                {
                    return code;
                }
            }
            String sql = "DELETE FROM @Table WHERE user_id = @UID AND course_id = @CID;";
            sql = sql.Replace("@Table", RoleToCourseTable[role]);
            return HandleNonQuery(sql, parameters);
        }
        public static (List<SubmitDate>,int,string) GetStudentExeriseDates(int dateID, string exerciseID){
            var dates = new List<SubmitDate>();
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (null,3,"Connection failed");}
            String sql = @"SELECT submission_date, reduction, group_number FROM Submission_Dates
                        WHERE submission_date_id = @DID OR (exercise_id = @EID AND group_number = 0)
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
        public static (int,DBCode) GetMainDate(string exerciseID){
            string sql = @"SELECT submission_date_id FROM Submission_Dates WHERE exercise_id=@EID AND group_number=0";
            SqlParameter[] parameters= { CreateParameter("@EID",System.Data.SqlDbType.VarChar, exerciseID) };
            return SingleRecordQuery(sql, parameters, (SqlDataReader reader) => reader.GetInt32(0));
        }
        public static DBCode CloseChat(string chatid){
            string sql = "UPDATE Chat SET chat_status=1 WHERE chat_id=@ID;";
            SqlParameter[] parameters = {
                CreateParameter("@ID",System.Data.SqlDbType.VarChar, chatid)
            };
            return HandleNonQuery(sql, parameters);
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
        public static (int,string) AddPasswordToken(string userid, string passwordToken, DateTime expiration){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = "IF EXISTS (SELECT 1 FROM Tokens WHERE user_id = @ID) INSERT INTO Tokens VALUES (@ID, @TOKEN, @DATE)";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@ID",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@TOKEN",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@DATE",System.Data.SqlDbType.DateTime);
            command.Parameters["@ID"].Value = userid;
            command.Parameters["@TOKEN"].Value = passwordToken;
             command.Parameters["@DATE"].Value = expiration;
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
        public static ((string, DateTime),int,string) ReadTokenInfo(string passwordToken) {
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
        public static (int, DBCode) AddExtension(SubmitDate date, string submissionId){
            String sql = LongQueries["AddExtension"];
            SqlParameter[] parameters = {
                CreateParameter("@SID",System.Data.SqlDbType.VarChar, submissionId),
                CreateParameter("@DATE",System.Data.SqlDbType.Date, date.Date),
                CreateParameter("@RE",System.Data.SqlDbType.Int, date.Reduction)
                //CreateParameter("@GR",System.Data.SqlDbType.Int, date.Group)
            };
            return IdentityInsert(sql, parameters);
            
        }
        public static (int, DBCode) IdentityInsert(string sql, SqlParameter[] parameters)
        {
            List<int> lst;
            try
            {
                lst = Query(sql, parameters, (SqlDataReader reader) => reader.GetInt32(0));
            }
            catch(Exception)
            {
                return (-1, DBCode.Error);
            }
            if(lst.Count == 0)
            {
                return (-1, DBCode.Error);
            }
            return (lst[0], DBCode.OK);
        }
        public static (List<RequestLabel>,DBCode) GetRequests(string exerciseID, ChatType type) {
            DateTime date = DateTime.MinValue;
            String sql = LongQueries["GetRequests"];
            SqlParameter[] parameters = {
                CreateParameter("@EID",System.Data.SqlDbType.VarChar, exerciseID),
                CreateParameter("@TYPE",System.Data.SqlDbType.Int, type)
            };
            return MultiRecordQuery(sql, parameters, (SqlDataReader dataReader) =>
                new RequestLabel {
                    ChatID = dataReader.GetValue(0).ToString(),
                    StudentID = dataReader.GetValue(1).ToString(),
                    StudentName = dataReader.GetValue(2).ToString(),
                    Message = dataReader.GetValue(3).ToString(),
                    Type = type
                }
            );
            
        }
        public static (List<RequestLabelMainPage>,DBCode) GetRequestsMainPage(string userID, ChatType type) {
            DateTime date = DateTime.MinValue;
            String sql = LongQueries["GetRequestsMainPage"];
            SqlParameter[] parameters = {
                CreateParameter("@ID",System.Data.SqlDbType.VarChar, userID),
                CreateParameter("@TYPE",System.Data.SqlDbType.Int, type)
            };
            return MultiRecordQuery(sql, parameters, (SqlDataReader dataReader) =>
                new RequestLabelMainPage {
                    CourseName = dataReader.GetValue(0).ToString(),
                    CourseNumber = dataReader.GetInt32(1),
                    CourseID = dataReader.GetValue(2).ToString(),
                    ExerciseID = dataReader.GetValue(3).ToString(),
                    ExerciseName = dataReader.GetValue(4).ToString(),
                    SubmissionID = dataReader.GetValue(5).ToString(),
                    ChatID = dataReader.GetValue(6).ToString(),
                    StudentName = dataReader.GetValue(7).ToString(),
                    Type = type
                }
            );
            
        }

        public static DBCode DeleteDate(int dateId){
            string sql = LongQueries["DeleteDate"];
            SqlParameter[] parameters =  { 
                CreateParameter("@ID",System.Data.SqlDbType.Int, dateId)
            };
            (int group, DBCode code) = SingleRecordQuery(sql, parameters, (SqlDataReader reader) => reader.GetInt32(0));
            if(code != DBCode.OK)
            {
                return code;
            }
            if(group == 0)
            {
                return DBCode.NotAllowed;
            }
            return DBCode.OK;
        }
       public static (int, DBCode) AddDate(SubmitDate date)
        {
            string sql = LongQueries["AddDate"];
            SqlParameter[] parameters =  { 
                CreateParameter("@ID",System.Data.SqlDbType.Int, date.ID),
                CreateParameter("@GR",System.Data.SqlDbType.Int, date.Group),
                CreateParameter("@DATE",System.Data.SqlDbType.Date, date.Date),
                CreateParameter("@RE",System.Data.SqlDbType.Int, date.Reduction)
            };
            (int id, DBCode code) = SingleRecordQuery(sql, parameters, (SqlDataReader reader) => reader.GetInt32(0));
            if(code != DBCode.OK)
            {
                return (-1, code);
            }
            if(id == -1)
            {
                return (id, DBCode.NotAllowed);
            }
            return (id, DBCode.OK);
        }
        public static DBCode UpDate(SubmitDate date){
            string sql = LongQueries["UpDate"];
            SqlParameter[] parameters =  { 
                CreateParameter("@ID",System.Data.SqlDbType.Int, date.ID),
                CreateParameter("@GR",System.Data.SqlDbType.Int, date.Group),
                CreateParameter("@DATE",System.Data.SqlDbType.Date, date.Date),
                CreateParameter("@RE",System.Data.SqlDbType.Int, date.Reduction)
            };
            (int group, DBCode code) = SingleRecordQuery(sql, parameters, (SqlDataReader reader) => reader.GetInt32(0));
            if(code != DBCode.OK)
            {
                return code;
            }
            if(group == 0)
            {
                return DBCode.NotAllowed;
            }
            return DBCode.OK;
        }
        public static (List<TeacherDateDisplay>,DBCode) GetDatseOfExercise(string exercise_id){
            List<TeacherDateDisplay> lst;
            var dict = new Dictionary<int, TeacherDateDisplay>();
            string sql = @"SELECT submission_date_id, submission_date, reduction, group_number
                                    FROM Submission_Dates WHERE exercise_id = @ID ORDER BY submission_date;";
            SqlParameter[] parameters =  {
                    CreateParameter("@ID",System.Data.SqlDbType.NVarChar, exercise_id)
            };
            try 
            {
                lst = Query<TeacherDateDisplay>(sql, parameters, (SqlDataReader dataReader) => {
                    var display = new TeacherDateDisplay {
                        ID = (int)dataReader.GetValue(0),
                        Date = (DateTime) dataReader.GetValue(1),
                        Reduction = (int)dataReader.GetValue(2),
                        Group = (int)dataReader.GetValue(3),
                    };
                    dict[display.ID] = display;
                    return display;
                });
                sql = LongQueries["SubmittersOfDate"];
                Query(sql, parameters, (SqlDataReader dataReader) => {
                    int id = (int)dataReader.GetValue(0);
                    var label = new UserLabel {
                        ID = dataReader.GetValue(1).ToString(),
                        Name =  dataReader.GetValue(2).ToString(),
                    };
                    var display = dict[id];
                    if(display.submitters == null)
                    {
                        display.submitters = new List<UserLabel>();
                    }
                    display.submitters.Add(label);
                });
            }
            catch
            {
                return (null, DBCode.Error);
            }
            return (lst, DBCode.OK);
        }  
        public static DBCode UpdateMoss(MossData data){
            string sql = @"UPDATE Exercise SET moss_matches=@SHOW, moss_stuff=@MAX moss_link=@LINK 
                            WHERE exercise_id = @ID;";
            SqlParameter[] parameters = {
                CreateParameter("@ID",System.Data.SqlDbType.VarChar, data.ExerciseID),
                new SqlParameter("@SHOW",System.Data.SqlDbType.Int) { Value = data.MatchesShow },
                new SqlParameter("@MAX",System.Data.SqlDbType.Int) { Value = data.MaxFound},
                new SqlParameter("@LINK",System.Data.SqlDbType.VarChar) { Value = data.Result }
            };
            return HandleNonQuery(sql, parameters);
        }    
        public static DBCode SetCopied(CopyForm copy) {
            string query = $"Update Submission SET has_copied=1 WHERE submission_id IN ANY (@ID1, @ID2)";
            SqlParameter[] parameters = {
                CreateParameter("@ID1", SqlDbType.VarChar, copy.user1),
                CreateParameter("@ID2", SqlDbType.VarChar, copy.user2),
                CreateParameter("@EID", SqlDbType.VarChar, copy.ExerciseID)
            };
            return HandleNonQuery(LongQueries["MarkCopied"], parameters);
        }
        public static DBCode BeginChecking(string submissionId, string userid) {
            string sql = "UPDATE Submission SET current_checker_id = null WHERE current_checker_id = @UID;";
            SqlParameter[] parameters = {
                CreateParameter("@UID",System.Data.SqlDbType.VarChar, userid)
            };
            DBCode code = HandleNonQuery(sql, parameters);
            if(code != DBCode.OK)
            {
                return code;
            }
            sql = "UPDATE Submission SET current_checker_id = @UID WHERE submission_id = @SID;";
            parameters = new SqlParameter[] { parameters[0], CreateParameter("@SID",System.Data.SqlDbType.VarChar, submissionId) };
            return HandleNonQuery(sql, parameters);
        }
         public static DBCode EndChecking(Submission submission) {
            String sql = @"UPDATE Submission SET manual_check_data = @MCD, submission_status = (CASE WHEN submission_status = 3 THEN 4 ELSE 2 END),
                            auto_grade = @AUTO, style_grade = @STYLE, manual_final_grade = @MANUAL, current_checker_id = null WHERE submission_id = @SID";
            SqlParameter[] parameters = {
                CreateParameter("@SID",System.Data.SqlDbType.VarChar, submission.ID),
                CreateParameter("@MCD",System.Data.SqlDbType.VarChar, submission.ManualCheckData),
                CreateParameter("@AUTO",System.Data.SqlDbType.Int, submission.AutoGrade),
                CreateParameter("@STYLE",System.Data.SqlDbType.Int, submission.StyleGrade),
                CreateParameter("@MANUAL",System.Data.SqlDbType.Int, submission.ManualFinalGrade)
            };
            return HandleNonQuery(sql, parameters);
        }

        public static (string, DBCode) GetMessageFile(int messageId)
        {
            string sql = "SELECT attached_file FROM Message WHERE message_id = @ID";
            SqlParameter[] parameters = { CreateParameter("@ID", SqlDbType.Int, messageId)};
            return SingleRecordQuery(sql, parameters, (SqlDataReader dataReader) => dataReader.GetValue(0)?.ToString());
        }
        public static (string, DBCode) GetTestDir(int testId)
        {
            string sql = "SELECT test_files_location FROM Tests WHERE test_id = @ID";
            SqlParameter[] parameters = { CreateParameter("@ID", SqlDbType.Int, testId)};
            return SingleRecordQuery(sql, parameters, (SqlDataReader dataReader) => dataReader.GetValue(0).ToString());
        }  
        public static (string, DBCode) GetSubDir(string submissionId)
        {
            string sql = "SELECT files_location FROM Submission WHERE submission_id = @ID";
            SqlParameter[] parameters = { CreateParameter("@ID", SqlDbType.VarChar, submissionId)};
            var a =  SingleRecordQuery(sql, parameters, (SqlDataReader dataReader) => dataReader.GetValue(0)?.ToString());
            return a;
        }  
        public static (string, DBCode) GetExDir(string exerciseId)
        {
            string sql = "SELECT test_files_location FROM Message WHERE message_id = @ID";
            SqlParameter[] parameters = { CreateParameter("@ID", SqlDbType.VarChar, exerciseId)};
            return SingleRecordQuery(sql, parameters, (SqlDataReader dataReader) => dataReader.GetString(0));
        }
        public static (List<CheckerExInfo>, DBCode) CheckerExercises(string checkerId)
        {
            string sql = LongQueries["CheckerExercises"];
            SqlParameter[] parameters = { CreateParameter("@ID", SqlDbType.VarChar, checkerId) };
            return MultiRecordQuery(sql, parameters, (SqlDataReader dataReader) =>
                new CheckerExInfo {
                    CourseName = dataReader.GetValue(0).ToString(),
                    CourseNumber = dataReader.GetInt32(1),
                    ExID = dataReader.GetValue(2).ToString(),
                    ExName = dataReader.GetValue(3).ToString(),
                    ToCheck = dataReader.GetInt32(4),
                    Appeals = dataReader.GetInt32(5)
                }
            );
        }  
         public static (int,string) UpdateTest(Test test){
            SqlConnection cnn  = new SqlConnection(connetionString);
            try{cnn.Open();} catch {return (3,"Connection failed");}
            String sql = @"UPDATE Test SET [weight]=@WEIGHT, input=@INPUT, expected_output=@EXPUT, output_file_name=@E,
                            arguments_string=@ARGS, timeout_in_seconds=@TIME, main_source_file=@MAIN, [type]=@TYPE) WHERE test_id=@TID";
            SqlCommand command = new SqlCommand(sql,cnn);
            command.Parameters.Add("@TID",System.Data.SqlDbType.Int);
            command.Parameters["@TID"].Value = test.ID;
            command.Parameters.Add("@TYPE",System.Data.SqlDbType.Int);
            command.Parameters.Add("@WEIGHT",System.Data.SqlDbType.Int);
            command.Parameters.Add("@INPUT",System.Data.SqlDbType.NText);
            command.Parameters.Add("@EOUTPUT",System.Data.SqlDbType.NText);
            command.Parameters.Add("@OFNAME",System.Data.SqlDbType.NVarChar);
            command.Parameters.Add("@ARGS",System.Data.SqlDbType.NText);
            command.Parameters.Add("@TIMEOUT",System.Data.SqlDbType.Int);
            command.Parameters.Add("@MAIN",System.Data.SqlDbType.NVarChar);
            command.Parameters["@TYPE"].Value = test.Type;
            command.Parameters["@WEIGHT"].Value = test.Value;
            command.Parameters["@INPUT"].Value = test.Input;
            command.Parameters["@EOUTPUT"].Value = test.Expected_Output;
            command.Parameters["@OFNAME"].Value = test.Output_File_Name;
            command.Parameters["@ARGS"].Value = test.ArgumentsString;
            command.Parameters["@TIMEOUT"].Value = test.Timeout_In_Seconds;
            command.Parameters["@MAIN"].Value = test.Main_Sourse_File;
            try{
                var reader = command.ExecuteNonQuery();
            } catch {
                try{cnn.Close();}catch{}
                return (2,"Addittion failed");
            }
            try{cnn.Close();} catch{return (4,"Connection close failed");}
            return (0,"OK");
        }
        public static DBCode DeleteTest(int testId){
            string sql = @"DELETE FROM Test WHERE test_id=@ID";
            SqlParameter[] parameters =  { 
                CreateParameter("@ID",System.Data.SqlDbType.Int, testId)
            };
            return HandleNonQuery(sql, parameters);
        }     
        public static DBCode DeleteSubmission(string submissionId){
            string sql = @"DELETE FROM Submission WHERE submission_id=@ID";
            SqlParameter[] parameters =  { 
                CreateParameter("@ID",System.Data.SqlDbType.VarChar, submissionId)
            };
            return HandleNonQuery(sql, parameters);
        }                        
    }
}