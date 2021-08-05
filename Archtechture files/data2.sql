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
DECLARE @sub1 AS VARCHAR(32) = N'89111-2021-ex1-576888433';
DECLARE @sub2 AS VARCHAR(32) = N'89111-2021-ex2-576888433';
DECLARE @sub3 AS VARCHAR(32) = N'89115-2021-ex1-123767888';
DECLARE @date1time AS DATETIME = DATEADD(day,14, GETDATE());
DECLARE @date2time AS DATETIME = DATEADD(day,28, GETDATE());
DECLARE @chat1 AS VARCHAR(32) = N'fjqewfojewog';
DECLARE @chat2 AS VARCHAR(32) = N'ewtfjqwpgfjq';
INSERT INTO Users VALUES (@Danny, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'דני דני', 'dana@gmail.com'),
                        (@Yosi, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'יוסי יוסי', 'yosi@gmail.com');

INSERT INTO Courses VALUES (@course_id1, 89111, @course_name1, 2021, 0),
                            (@course_id2, 89112, @course_name2, 2021, 0),
                            (@course_id3, 89115, @course_name3, 2021, 0);

INSERT INTO Student_Course VALUES (@Yosi,@course_id1), (@Yosi, @course_id2), (@Danny, @course_id3);

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