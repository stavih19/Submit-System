IF DB_ID('submit02') IS NULL
   CREATE DATABASE submit02;
GO
USE submit02;
DECLARE @sql NVARCHAR(max)=''
SELECT @sql += ' Drop table ' + QUOTENAME(TABLE_SCHEMA) + '.'+ QUOTENAME(TABLE_NAME) + '; '
FROM   INFORMATION_SCHEMA.TABLES
WHERE  TABLE_TYPE = 'BASE TABLE'
Exec Sp_executesql @sql
GO
CREATE TABLE Users (
    user_id nvarchar(10), 
    password_hash nvarchar(64),
    name nvarchar(32),
    email nvarchar(64)
);
CREATE TABLE Courses (
    course_id nvarchar(16),
    course_number int,
    course_name nvarchar(32),
    year int,
    semester int 
);
CREATE TABLE Student_Course(
    user_id nvarchar(10),
    course_id nvarchar(16)
);
CREATE TABLE Metargel_Course(
    user_id nvarchar(10),
    course_id nvarchar(16)
);
CREATE TABLE Checker_Course(
    user_id nvarchar(10),
    course_id nvarchar(16)
);
CREATE TABLE Exercise(
    exercise_id nvarchar(48),
    exercise_name nvarchar(32),
    course_id nvarchar(16),
    original_exercise_id nvarchar(48),
    max_submitters int,
    test_files_location nvarchar(64),
    late_submission_settings nvarchar(64),
    programming_language nvarchar(32),
    auto_test_grade_value int,
    style_test_grade_value int,
    is_active int,
    multiple_submissions int,
    moss_number_of_matches int,
    moss_max_match int,
    moss_result_link varchar(256),
    creation DATETIME
);
CREATE TABLE Checker_Exercise(
    user_id nvarchar(10),
    exercise_id nvarchar(48)
);

CREATE TABLE Submission_Dates(
    exercise_id nvarchar(48),
    submission_date_id int IDENTITY(1, 1),
    submission_date DATE,
    reduction int,
    group_number int
);
CREATE TABLE Submission(
    submission_id nvarchar(60),
    exercise_id nvarchar(48),
    files_location nvarchar(64),
    auto_grade int,
    style_grade int,
    manual_final_grade int,
    manual_check_data nvarchar(255),
    submission_status int,
    submission_date_id int,
    time_submitted DATETIME,
    has_copied int,
    current_checker_id varchar(10)
);

CREATE TABLE Submitters(
    user_id nvarchar(10),
    submission_id nvarchar(60),
    submitter_type int,
    exercise_id nvarchar(48)
);
CREATE TABLE Chat(
    chat_id nvarchar(60),
    submission_id nvarchar(60),
    chat_status int,
    chat_type int
);
CREATE TABLE Message(
    message_id int IDENTITY(1,1) ,
    chat_id nvarchar(60),
    message_time DATETIME DEFAULT GETDATE(),
    attached_file varchar(128),
    message_text ntext,
    sender_user_id nvarchar(10),
    message_status int,
    course_id nvarchar(16),
    from_teacher int,
    sender_name nvarchar(64),
);
CREATE TABLE Managers(
    id nvarchar(32)
);

CREATE TABLE Test(
    test_id int IDENTITY(1, 1),
    [weight] int,
    input ntext,
    expected_output ntext,
    output_file_name nvarchar(32),
    arguments_string ntext,
    timeout_in_seconds int,
    main_source_file nvarchar(32),
    additional_files_location nvarchar(64),
    exercise_id nvarchar(48),
    [type] int
);

CREATE TABLE Tokens(
    user_id varchar(10),
    token varchar(64),
    expiry_date DATETIME
);
GO
CREATE TRIGGER DELETE_DATE
ON Submission_Dates
AFTER DELETE
AS
    UPDATE Submission SET submission_date_id = null FROM Submission AS S INNER JOIN Deleted ON S.submission_date_id = Deleted.submission_date_id 