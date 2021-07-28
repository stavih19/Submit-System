CREATE DATABASE Submit02;
GO
USE Submit02;
CREATE TABLE Users (
    user_id nvarchar(10),
    password_hash nvarchar(48),
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
    multiple_submissions int
);
CREATE TABLE Checker_Exercise(
    user_id nvarchar(10),
    exercise_id nvarchar(48)
);

CREATE TABLE Submission_Dates(
    submission_date_id int,
    exercise_id nvarchar(48),
    reduction int,
    [group] int,
    submission_date DATE
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
    chat_late_id nvarchar(32),
    chat_appeal_id nvarchar(32)
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
    message_id int IDENTITY(1,1),
    chat_id nvarchar(60),
    message_time DATETIME,
    message_type int,
    message_value nvarchar(8000),
    sender_user_id nvarchar(10),
    message_status int,
    course_id nvarchar(16),
    from_teacher int,
    sender_name nvarchar(64),
);
CREATE TABLE Managers(
    id nvarchar(32),
    password_hash nvarchar(64)
);

CREATE TABLE Test(
    test_id int IDENTITY(1, 1),
    [weight] int,
    input ntext,
    expected_output ntext,
    output_file_name nvarchar(32),
    arguments_string ntext,
    timeout_in_seconds int,
    main_sourse_file nvarchar(32),
    additional_files_location nvarchar(64),
    exercise_id nvarchar(48),
    [type] int
);

CREATE TABLE Tokens(
    user_id varchar(10),
    token varchar(64),
    expiry_date DATETIME
);