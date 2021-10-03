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
DECLARE @exercise4 AS VARCHAR(32) = N'89115_2021_ex0';
DECLARE @sub1 AS VARCHAR(32) = N'576888433_89111_2021_ex1';
DECLARE @sub35 AS VARCHAR(32) = N'576888444_89115_2021_ex1';
DECLARE @sub32 AS VARCHAR(32) = N'250283520_89115_2021_ex1';
DECLARE @sub2 AS VARCHAR(32) = N'576888433_89111_2021_ex2';
DECLARE @sub3 AS VARCHAR(32) = N'123767888_89115_2021_ex1';
DECLARE @sub4 AS VARCHAR(32) = N'123767888_89115_2021_ex0';
DECLARE @date1time AS DATETIME = DATEADD(day,14, GETDATE());
DECLARE @date2time AS DATETIME = DATEADD(day,28, GETDATE());
DECLARE @chat1 AS VARCHAR(32) = N'B576888433_89111_2021_ex1';
DECLARE @chat2 AS VARCHAR(32) = N'A576888433_89111_2021_ex2';
DECLARE @chat3 AS VARCHAR(32) = N'A123767888_89115_2021_ex1';
DECLARE @chat4 AS VARCHAR(32) = N'B123767888_89115_2021_ex0';
DECLARE @Tal AS VARCHAR(10) = '184140232';
DECLARE @Amit AS VARCHAR(10) = '250283520';
DECLARE @Dana VARCHAR(32) = N'576888444';
DECLARE @Alon VARCHAR(32) = N'248202842';
DECLARE @Tomer VARCHAR(32) = N'240812481';
INSERT INTO Users VALUES (@Danny, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'דני דני', 'dana@gmail.com'),
                        (@Yosi, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'יוסי יוסי', 'yosi@gmail.com'),
                        (@Tal, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'טל טל', 'Tal@gmail.com'),
                        (@Amit, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'עמית עמית', 'Amit@gmail.com'),
                        (@Dana, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'דנה דנה', 'Dana@gmail.com'),
                        (@Tomer, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'תומר תומר', 'tomer@gmail.com');

INSERT INTO Courses VALUES (@course_id1, 89111, @course_name1, 2021, 0),
                            (@course_id2, 89112, @course_name2, 2021, 0),
                            (@course_id3, 89115, @course_name3, 2021, 0);

INSERT INTO Student_Course VALUES (@Yosi,@course_id1), (@Yosi, @course_id2), (@Danny, @course_id3), (@Dana, @course_id3), (@Alon, @course_id1),
                                (@Amit, @course_id3)

INSERT INTO Checker_Course VALUES (@Tal, @course_id1), (@Tal, @course_id2), (@Tal, @course_id3),
                                (@Tomer, @course_id1), (@Tomer, @course_id2), (@Tomer, @course_id3), (@Yosi,@course_id3);

INSERT INTO Checker_Exercise VALUES (@Tomer, @exercise1), (@Tomer, @exercise2), (@Yosi, @exercise3), (@Yosi, @exercise4);

INSERT INTO Metargel_Course VALUES (@Yosi,@course_id3), (@Danny,@course_id1);
INSERT INTO Exercise VALUES (@exercise1, 'ex1', @course_id1, '?', 2, 'Courses\89111_2021\Exercises\ex1\', '10_20', 'python3', 60, 20, 1, 1, 250, 10, null, GETDATE()),
                            (@exercise2, 'ex2', @course_id1, '?', 2, 'Courses\89111_2021\Exercises\ex2\', '10_20', 'python3', 60, 20, 1, 1, 250, 10, null, GETDATE()),
                            (@exercise4, 'ex0', @course_id3, '?', 2, 'Courses\89115_2021\Exercises\ex0\', '10_20', 'python3', 60, 20, 1, 1, 250, 10, null, GETDATE()),
                            (@exercise3, 'ex1', @course_id3, '?', 2, 'Courses\89115_2021\Exercises\ex1\', '10_20', 'python3', 60, 20, 1, 1, 250, 10, null, GETDATE());


INSERT INTO Submission_Dates(exercise_id, submission_date, reduction, group_number)
VALUES (@exercise1, @date1time, 0, 0);
DECLARE @date1 as int = IDENT_CURRENT('Submission_Dates');

INSERT INTO Submission_Dates(exercise_id, submission_date, reduction, group_number)
VALUES (@exercise2, @date1time, 0, 0);
DECLARE @date2 as int = IDENT_CURRENT('Submission_Dates');

INSERT INTO Submission_Dates(exercise_id, submission_date, reduction, group_number)
VALUES (@exercise3, @date1time, 0, 0);
DECLARE @date3 as int = IDENT_CURRENT('Submission_Dates');

INSERT INTO Submission Values (@sub1, @exercise1, 'Courses\89111_2021\Exercises\ex1\Submissions\576888433', 100, 100, 100, '?', 2, @date1, GETDATE(), 0, null),
                                (@sub35, @exercise3, 'Courses\89115_2021\Exercises\ex1\Submissions\' + @Dana, -1, -1, -1, '?', 2, @date1, GETDATE(), 0, null),
                                (@sub32, @exercise3, 'Courses\89115_2021\Exercises\ex1\Submissions\' + @Amit, -1, -1, -1, '?', 3, @date1, GETDATE(), 0, null),
                                (@sub2, @exercise2, 'Courses\89111_2021\Exercises\ex2\Submissions\576888433', -1,-1, -1, '?', 0, @date2, null , 0, null),
                                (@sub3, @exercise3, 'Courses\89115_2021\Exercises\ex1\Submissions\123767888', -1,-1, -1, '?', 1, @date3, null, 0, null),
                                (@sub4, @exercise4, 'Courses\89115_2021\Exercises\ex0\Submissions\123767888', 100,100, 100, '?', 0, @date3, GETDATE(), 0, null)

INSERT INTO Submitters VALUES (@Yosi, @sub1, 1, @exercise1),
                                (@Dana, @sub35, 1, @exercise3),
                                (@Amit, @sub32, 1, @exercise3),
                                (@Yosi, @sub2, 1, @exercise2),
                                (@Danny, @sub3, 1, @exercise3),
                                (@Danny, @sub4, 1, @exercise4);
INSERT INTO Chat VALUES (@chat1, @sub1, 1, 1), (@chat2, @sub2, 0, 0), (@chat3, @sub3, 0, 0), (@chat4, @sub4, 0, 1);
INSERT INTO Message
    (chat_id,attached_file,message_text,sender_user_id,message_status,course_id,
     from_teacher, sender_name)
    VALUES (@chat1,null,N'אני רוצה הארכה',@Yosi,0,'?', 0, N'יוסי יוסי'),
            (@chat1,null,N'אוקי',@Danny,0,'?', 1, N'יוסי יוסי'),
            (@chat2,null,N'אני רוצה לערער',@Yosi,0,'?', 0, N'יוסי יוסי'),
            (@chat2,null,N'אוקי',@Danny,0,'?', 1, N'דני דני'),
            (@chat3,null,N'אני רוצה הארכה',@Danny,0,'?', 0, N'דני דני'),
            (@chat4,null,N'אני רוצה לערער',@Danny,0,'?', 0, N'דני דני');
INSERT INTO Test([weight], input, expected_output, output_file_name, arguments_string,
        timeout_in_seconds, main_source_file, additional_files_location, exercise_id, type)
VALUES (100, '', 'The number is 50*50=2500', 'stdout', '50', 60, 'aaa.py', null, @exercise1, 0),
        (100, '', 'The number is 50*50=2500', 'stdout', '50', 60, 'aaa.py', null, @exercise1, 1),
        (100, '', 'The number is 50*50=2500', 'stdout', '50', 60, 'aaa.py', null, @exercise2, 0),
        (100, '', 'The number is 50*50=2500', 'stdout', '50', 60, 'aaa.py', null, @exercise2, 1),
        (100, '', 'Hello world', 'stdout', '', 60, 'ex1.py', 'Courses\89115_2021\Exercises\ex1\Runfiles\Test_1', @exercise3, 0),
        (100, '', 'Hello world', 'stdout', '', 60, 'ex1.py', 'Courses\89115_2021\Exercises\ex1\Runfiles\Test_2', @exercise3, 1);
GO