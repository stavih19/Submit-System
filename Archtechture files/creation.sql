CREATE DATABASE Submit02;
GO
USE Submit02;
CREATE TABLE Users (
    user_id varchar(10),
    password_hash varchar(32),
    name varchar(32),
    email varchar(64)
);
CREATE TABLE Courses (
    course_id varchar(16),
    course_number int,
    course_name varchar(32),
    year int,
    semester int 
);
CREATE TABLE Student_Course(
    user_id varchar(10),
    course_id varchar(16)
);
CREATE TABLE Metargel_Course(
    user_id varchar(10),
    course_id varchar(16)
);
CREATE TABLE Exercise(
    exercise_id varchar(48),
    exercise_name varchar(32),
    course_id varchar(16),
    original_exercise_id varchar(48),
    max_submitters int,
    test_files_location varchar(64),
    late_submittion_settings varchar(64),
    programming_language varchar(32),
    auto_test_grade_value int,
    style_test_grade_value int,
    is_active int
);
CREATE TABLE Checker_Exercise(
    user_id varchar(10),
    exercise_id varchar(48)
);
CREATE TABLE Submission_Dates(
    exercise_id varchar(48),
    submission_date_id int,
    submission_date DATE
);
CREATE TABLE Submission(
    submission_id varchar(32),
    exercise_id varchar(48),
    files_location varchar(64),
    auto_grade int,
    style_grade int,
    manual_final_grade int,
    manual_check_data varchar(255),
    submission_status varchar(10),
    submission_date_id int,
    time_submitted DATETIME,
    chat_late_id varchar(32),
    chat_appeal_id varchar(32)
);
CREATE TABLE Submitters(
    user_id varchar(10),
    submission_id varchar(32),
    submitter_type int
);
CREATE TABLE Chat(
    chat_id varchar(32),
    submission_id varchar(32),
    chat_status int,
    chat_type int
);
CREATE TABLE Massage(
    chat_id varchar(32),
    massage_number int,
    massage_time DATETIME,
    massage_type int,
    massage_value varchar(255),
    sender_user_id varchar(10),
    massage_status int,
    course_id varchar(16),
);
CREATE TABLE Managers(
    id varchar(32),
    password_hash varchar(32)
);