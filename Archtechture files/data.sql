-- Empty tables
EXEC sp_MSForEachTable 'DISABLE TRIGGER ALL ON ?'
GO
EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
GO
EXEC sp_MSForEachTable 'DELETE FROM ?'
GO
EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
GO
EXEC sp_MSForEachTable 'ENABLE TRIGGER ALL ON ?'
GO
DECLARE @Dana VARCHAR(32) = N'576888444';
DECLARE @Yosi VARCHAR(32) = N'576888433';
DECLARE @Danny AS VARCHAR(32) = N'123767888';
DECLARE @course_name1 AS NVARCHAR(32) = N'תכנות מתקדם 1';
DECLARE @course_name2 AS NVARCHAR(32) = N'מבוא לרשתות תקשורת';
DECLARE @course_name3 AS NVARCHAR(32) = N'מבנה מחשב';
DECLARE @course_id1 AS VARCHAR(32) = N'89111_2021';
DECLARE @course_id2 AS VARCHAR(32) = N'89112_2021';
DECLARE @course_id3 AS VARCHAR(32) = N'89115_2021';
DECLARE @exercise1 AS VARCHAR(32) = N'89111_2021_ex1';
DECLARE @exercise2 AS VARCHAR(32) = N'89111_2021_ex2';
DECLARE @exercise3 AS VARCHAR(32) = N'89115_2021_ex1';
DECLARE @sub1 AS VARCHAR(32) = N'89111_2021_ex1_576888433';
DECLARE @sub15 AS VARCHAR(32) = N'89111_2021_ex1_576888444';
DECLARE @sub2 AS VARCHAR(32) = N'89111_2021_ex2_576888433';
DECLARE @sub3 AS VARCHAR(32) = N'89115_2021_ex1_123767888';
DECLARE @date1time AS DATETIME = DATEADD(day,14, GETDATE());
DECLARE @date2time AS DATETIME = DATEADD(day,28, GETDATE());
DECLARE @chat1 AS VARCHAR(32) = N'fjqewfojewog';
DECLARE @chat2 AS VARCHAR(32) = N'ewtfjqwpgfjq';
INSERT INTO Users VALUES (@Danny, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'דני דני', 'dana@gmail.com'),
                        (@Yosi, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'יוסי יוסי', 'yosi@gmail.com'),
                        (@Dana, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'דנה דני', 'yosi@gmail.com');

INSERT INTO Courses VALUES (@course_id1, 89111, @course_name1, 2021, 0),
                            (@course_id2, 89112, @course_name2, 2021, 0),
                            (@course_id3, 89115, @course_name3, 2021, 0);

INSERT INTO Student_Course VALUES (@Yosi,@course_id1), (@Yosi, @course_id2), (@Danny, @course_id3), (@Dana, @course_id1);

INSERT INTO Metargel_Course VALUES (@Yosi,@course_id3), (@Danny,@course_id1);
INSERT INTO Exercise VALUES (@exercise1, 'ex1', @course_id1, '?', 1, 'Courses\89111_2021\Exercises\ex1\', '10_20', 'python', 60, 20, 1, 1, 250, 10, null, GETDATE()),
                            (@exercise2, 'ex2', @course_id1, '?', 1, 'Courses\89111_2021\Exercises\ex2\', '10_20', 'python', 60, 20, 1, 1, 250, 10, null, GETDATE()),
                            (@exercise3, 'ex1', @course_id3, '?', 1, 'Courses\89115_2021\Exercises\ex1\', '10_20', 'python', 60, 20, 1, 1, 250, 10, null, GETDATE());


INSERT INTO Submission_Dates(exercise_id, submission_date, reduction, group_number)
VALUES (@exercise1, @date1time, 0, 0);
DECLARE @date1 as int = IDENT_CURRENT('Submission_Dates');

INSERT INTO Submission_Dates(exercise_id, submission_date, reduction, group_number)
VALUES (@exercise2, @date1time, 0, 0);
DECLARE @date2 as int = IDENT_CURRENT('Submission_Dates');

INSERT INTO Submission_Dates(exercise_id, submission_date, reduction, group_number)
VALUES (@exercise3, @date1time, 0, 0);
DECLARE @date3 as int = IDENT_CURRENT('Submission_Dates');

INSERT INTO Submission Values (@sub1, @exercise1, 'Courses\89111_2021\Exercises\ex1\Submissions\576888433', 100, 100, 100, '?', 3, @date1, GETDATE(), 0, null),
                                (@sub15, @exercise1, 'Courses\89111_2021\Exercises\ex1\Submissions\576888433', 100, 100, 100, '?', 3, @date1, GETDATE(), 0, null),
                                (@sub2, @exercise2, 'Courses\89111_2021\Exercises\ex2\Submissions\576888433', -1,-1, -1, '?', 0, @date2, null , 0, null),
                                (@sub3, @exercise3, 'Courses\89115_2021\Exercises\ex1\Submissions\123767888', -1,-1, -1, '?', 0, @date3, null, 0, null);

INSERT INTO Submitters VALUES (@Yosi, @sub1, 1, @exercise1), (@Yosi, @sub2, 1, @exercise2), (@Danny, @sub3, 1, @exercise3),  (@Dana, @sub15, 1, @exercise1);
INSERT INTO Chat VALUES (@chat1, @sub1, 1, 1), (@chat2, @sub2, 0, 0);
INSERT INTO Message
    (chat_id,attached_file,message_text,sender_user_id,message_status,course_id,
     from_teacher, sender_name)
    VALUES (@chat1,null,N'אני רוצה הארכה',@Yosi,0,'?', 0, N'יוסי יוסי'),
            (@chat1,null,N'אוקי',@Danny,0,'?', 1, N'יוסי יוסי'),
            (@chat2,null,N'אני רוצה לערער',@Yosi,0,'?', 0, N'יוסי יוסי'),
            (@chat2,null,N'אוקי',@Danny,0,'?', 1, N'דני דני');
INSERT INTO Test([weight], input, expected_output, output_file_name, arguments_string,
        timeout_in_seconds, main_source_file, additional_files_location, exercise_id, type)
VALUES (100, '', 'the number is 5*50=250', 'stdout', '50', 60, 'aaa.py', null, @exercise1, 0)
GO