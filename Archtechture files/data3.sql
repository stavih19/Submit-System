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
DECLARE @Tal AS VARCHAR(10) = '184140232';
DECLARE @Amit AS VARCHAR(10) = '250283520';
DECLARE @Dana VARCHAR(32) = N'576888444';
INSERT INTO Users VALUES (@Danny, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'דני דני', 'dana@gmail.com'),
                        (@Yosi, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'יוסי יוסי', 'yosi@gmail.com'),
                        (@Tal, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'טל טל', 'Tal@gmail.com'),
                        (@Amit, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'עמית עמית', 'Amit@gmail.com'),
                        (@Dana, 'IjHzX+F5umurQ2u7ADFzi6qYz4U=$Jwv9yeDBSZw=$1000', N'דנה דנה', 'Dana@gmail.com');

INSERT INTO Courses VALUES (@course_id1, 89111, @course_name1, 2021, 0),
                            (@course_id2, 89112, @course_name2, 2021, 0),
                            (@course_id3, 89115, @course_name3, 2021, 0);

INSERT INTO Student_Course VALUES (@Yosi,@course_id1), (@Yosi, @course_id2), (@Danny, @course_id3), (@Dana, @course_id1);

INSERT INTO Checker_Course VALUES (@Tal, @course_id1), (@Tal, @course_id2);

INSERT INTO Metargel_Course VALUES (@Yosi,@course_id3), (@Danny,@course_id1);