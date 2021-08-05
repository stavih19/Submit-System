-- Uses 3 consecutive hyphens to separate the page into keys and queries
---StudentDates---
WITH DS AS
(SELECT S.submission_id, MAX(D.submission_date) AS submission_date
FROM Submitters AS SS
INNER JOIN 
Submission AS S
ON SS.user_id = @ID
AND SS.submission_id = S.submission_id
INNER JOIN Submission_Dates AS D
ON ((D.submission_date_id = S.submission_date_id OR group_number = 0) AND reduction = 0)
GROUP BY S.submission_id)
SELECT C.course_id,
    C.course_name, 
    C.course_number,
    E.exercise_id,
    E.exercise_name,
    DS.submission_date,
    S.submission_date_id
FROM Submitters as SS
    INNER JOIN Submission as S 
        ON S.submission_id = SS.submission_id AND S.submission_status = 0 AND SS.user_id = @ID
    INNER JOIN Exercise as E
        ON S.exercise_id = E.exercise_id
    INNER JOIN Courses as C
        ON C.course_id = E.course_id
    INNER JOIN DS
        ON DS.submission_id = S.submission_id
ORDER BY DS.submission_date
---StudentGrades---
--Selects everything needed to calcualte the student's grades
SELECT
    C.course_id,
    C.course_name,
    C.course_number,
    E.exercise_id,
    E.exercise_name,
    E.auto_test_grade_value,
    E.style_test_grade_value,
    S.time_submitted,
    E.late_submission_settings,
    S.auto_grade,
    S.style_grade,
    S.manual_final_grade
FROM Submitters as SS
    INNER JOIN Submission as S
        ON S.submission_id = SS.submission_id AND SS.user_id = @ID AND S.submission_status > 1
        AND DATEDIFF(year, GETDATE(), S.time_submitted ) > 0
    INNER JOIN Exercise as E
        ON S.exercise_id = E.exercise_id
    INNER JOIN Courses as C
        ON C.course_id = E.course_id
ORDER BY S.time_submitted
---StudentGrades2---
SELECT 
    E.exercise_id, D.submission_date, D.reduction
FROM Submitters as SS
    INNER JOIN Submission as S
        ON S.submission_id = SS.submission_id AND SS.user_id = @ID AND S.submission_status > 1
    INNER JOIN Exercise as E
        ON S.exercise_id = E.exercise_id
    INNER JOIN Submission_Dates as D
            ON S.submission_date_id = D.submission_date_id OR (D.group_number = 0 AND E.exercise_id = D.exercise_id)
---AllExercises---
-- Given a course id, returns the exercises of the course for every year
SELECT C.year,
    E.exercise_id,
    E.exercise_name
FROM Courses AS C
    LEFT JOIN Exercise as E 
        ON C.course_id = E.course_id
WHERE C.course_number =
    (SELECT course_number from Courses WHERE course_id = @CID)
ORDER BY E.exercise_id DESC
---Courses---
SELECT
    C.course_id,
    C.course_name,
    C.course_number,
    C.year,
    C.semester 
FROM @Table AS UC 
    INNER JOIN Courses AS C
        ON C.course_id = UC.course_id
WHERE UC.user_id = @ID
ORDER BY C.year, C.semester, C.course_number DESC
---UsersCourse---
SELECT
    U.user_id,
    U.name
FROM @Table AS UC 
    INNER JOIN Users as U
        ON UC.user_id = U.user_id
        AND UC.course_id = @CID
---ExerciseCheckers---
SELECT
    U.user_id,
    U.name
FROM Checker_Exercise AS CE 
    INNER JOIN Users as U
        ON CE.user_id = U.user_id
        AND CE.exercise_id = @EID
---SubmissionLabels---
SELECT
    S.submission_id, SS.user_id, U.name, S.submission_status
FROM Submission AS S
    INNER JOIN Submitters as SS
        ON SS.submission_id = S.submission_id
    INNER JOIN Users as U
        ON SS.user_id = U.user_id
WHERE S.exercise_id = @ID AND S.submission_status != 0
---HasExercise---
SELECT 1
FROM @Table AS UC
    INNER JOIN Courses as C 
        ON UC.course_id = C.course_id
        AND UC.user_id = @UID
    INNER JOIN Exercise AS E
        ON E.course_id = C.course_id
        AND E.exercise_id = @RID
---HasExerciseChecker---
SELECT 1 FROM Chercker_Exercise WHERE user_id = @UID AND exercise_id = @EID
---HasSubmissionChecker---
 SELECT 1 FROM Submission AS S
    INNER JOIN Exercise_Checker AS EC
        ON S.exercise_id = EC.exercise_id
        AND EC.user_id = @UID
        AND S.submisison_id = @RID
---HasSubmissionTeacher---
SELECT 1
FROM Submission as S
    INNER JOIN Exercise AS E
       ON E.exercuse_id = S.exercise_id
        AND S.submission_id = @RID
    INNER JOIN Metargel_Course AS UC
        ON UC.course_id = E.course_id
        AND UC.user_id = @UID
---HasSubmissionStudent---
SELECT 1 
FROM Submission AS S
    INNER JOIN Submitters AS SS
        ON S.submission_id = SS.submission_id
        AND S.submission_id = @RID
        AND SS.user_id = @UID
---HasChatTeacher---
SELECT 1
FROM Chat as Ch 
INNER JOIN Submission as S
        ON Ch.chat_id = @RID
        AND Ch.submission_id = S.submission_id
    INNER JOIN Exercise AS E
       ON E.exercuse_id = S.exercise_id
    INNER JOIN Metargel_Course AS UC
        ON UC.course_id = E.course_id
        AND UC.user_id = @UID
---HasChatStudent---
SELECT 1 
FROM Chat AS C
    INNER JOIN Submitters as SS
        ON SS.submission_id = C.submission_id
        AND SS.user_id = @UID
        AND C.chat_id = @RID
---HasDate---
SELECT 1
FROM Submisison_Dates as D
    INNER JOIN Exercise AS E
       ON E.exercuse_id = D.exercise_id
       AND D.submission_date_id = @RID
    INNER JOIN Metargel_Course AS MC
        ON UC.course_id = E.course_id
        AND UC.user_id = @UID
---HasTest---
SELECT 1
FROM Tests as T
    INNER JOIN Exercise AS E
       ON E.exercuse_id = T.exercise_id
       AND T.test_id = @RID
    INNER JOIN Metargel_Course AS MC
        ON UC.course_id = E.course_id
        AND UC.user_id = @UID
---GetRequests---
WITH FM (id) AS (SELECT MIN(message_id) FROM [Message] GROUP BY chat_id)
SELECT
    C.chat_id,
    M.sender_user_id,
    M.sender_name,
    M.message_text
FROM Chat AS C
    INNER JOIN Submission AS S
        ON C.submission_id = S.submission_id
        AND C.chat_type=@TYPE
        AND S.exercise_id = @EID
    INNER JOIN [Message] AS M
        ON C.chat_id = M.chat_id
    INNER JOIN [FM]
        ON FM.id = M.message_id
---TeacherExercises---
SELECT
    C.course_name,
    C.course_number,
    C.course_id, 
    E.exercise_id,
    E.exercise_name
FROM Metargel_Course AS MC
    INNER JOIN Courses AS C
        ON MC.user_id=@ID AND MC.course_id = C.course_id
    INNER JOIN Exercise AS E
        ON E.course_id = C.course_id
ORDER BY E.exercise_name DESC
---DeleteChecker---
DELETE FROM Checker_Exercise
WHERE user_id = @UID
AND exercise_id IN(SELECT exercise_id FROM Exercise WHERE course_id = @CID)
---SubmittersOfDate---
SELECT
    D.submission_date_id, 
    SS.user_id, 
    U.name
FROM Submission_Dates AS D
    INNER JOIN Submission as S
        ON D.submission_date_id = S.submission_date_id
        AND D.exercise_id = @ID
        AND D.group_number != 0
    INNER JOIN Submitters AS SS
        ON SS.submission_id = S.submission_id
    INNER JOIN Users as U
        ON SS.user_id = U.user_id
---AddDate---
 IF EXISTS (SELECT 1 FROM Submission_Dates WHERE exercise_id = @EID AND group_number = @GR AND @GR != -1)
        SELECT -1
    ELSE
        INSERT INTO Submission_Dates(exercise_id, submission_date, reduction, group_number)
        OUTPUT Inserted.submission_date_id
        VALUES (@EID, @DATE, @RE, @GR);
---UpDate---
UPDATE  Submission_Dates
SET 
    submission_date = @DATE,
    reduction = (CASE WHEN group_number = 0 THEN 0 ELSE @RE END)
OUTPUT group_number
WHERE submission_date_id = @ID
---DeleteDate---
DELETE FROM Submission_Dates
OUTPUT (SELECT group_number FROM Submission_Dates WHERE submission_date_id = @ID)
WHERE submission_date_id = @ID AND group_number != 0
----Copied---
UPDATE S SET S.has_copied = 1
FROM Submission AS S INNER JOIN
(SELECT DISTINCT S.submission_id AS id
    FROM Submission AS S
        INNER JOIN Submitters AS SS
        ON S.submission_id = SS.submission_id 
        AND S.exercise_id = @EID
        AND SS.user_id IN (@ID1, @ID2)
        GROUP BY S.submission_id
        HAVING COUNT(*) = 2)
 AS Subs ON Subs.id = S.submission_id;
---HasMessage---
SELECT 1 FROM Message AS M
INNER JOIN Chat AS C
ON M.chat_id = C.chat_id
AND M.message_id = @RID
INNER JOIN Submission AS S
ON C.submission_id = S.submission_id
INNER JOIN Exercise AS E
ON E.exercise_id = S.exercise_id
INNER JOIN @Table AS UC
ON UC.course_id = E.course_id
AND UC.user_id = @UID
---CheckerExercises---
SELECT
    C.course_name,
    C.course_number,
    S.exercise_id,
    E.exercise_name,
    SUM(case when submission_status = 1 then 1 else 0 end) AS to_check, 
    SUM(case when submission_status = 3 then 1 else 0 end) AS appeals 
FROM Submission AS S
    INNER JOIN Checker_Exercise AS CE
        ON CE.exercise_id = S.exercise_id AND CE.user_id = @ID
    INNER JOIN Exercise AS E
         ON E.exercise_id = S.exercise_id
    INNER JOIN Courses AS C
        ON C.course_id = E.course_id
GROUP BY S.exercise_id, E.exercise_name, C.course_name,  C.course_number
---HasOldExercise---
SELECT 1 FROM Exercise AS E
INNER JOIN Courses AS C
ON E.exercise_id = @EID
AND E.course_id = C.course_id
AND C.course_number IN
(SELECT C.course_number
 FROM Courses
    INNER JOIN Metargel_Course AS MC
    ON MC.user_id = @UID
    AND MC.course_id = C.course_id)
---HasOldTest---
SELECT 1 FROM Test AS T
INNER JOIN Exercise AS E
ON T.test_id = @TID
AND T.exercise_id = E.exercise_id
INNER JOIN Courses AS C
ON E.course_id = C.course_id
AND C.course_number IN
(SELECT C.course_number
 FROM Courses
    INNER JOIN Metargel_Course AS MC
    ON MC.user_id = @UID
    AND MC.course_id = C.course_id)
---GetRequestsMainPage---
WITH FM (id) AS (SELECT MIN(message_id) FROM [Message] GROUP BY chat_id)
SELECT
    Co.course_name,
    Co.course_number,
    Co.course_id,
    E.exercise_id,
    E.exercise_name,
    S.submission_id,
    C.chat_id,
    M.sender_name
    FROM Metargel_Course AS MC
    INNER JOIN Courses AS Co
    ON MC.user_id = @ID
    AND Co.course_id = MC.course_id
    INNER JOIN Exercise AS E
    ON E.course_id = Co.course_id
    INNER JOIN Submission AS S
    ON E.exercise_id = S.exercise_id
    INNER JOIN Chat AS C
        ON C.submission_id = S.submission_id
        AND C.chat_type=@TYPE
    INNER JOIN [Message] AS M
        ON C.chat_id = M.chat_id
    INNER JOIN [FM]
        ON FM.id = M.message_id
---AddToken---
BEGIN TRAN
    IF EXISTS(SELECT 1 FROM Tokens WHERE user_id=@ID)
        UPDATE Tokens SET token = @TOKEN, expiry_date = @EXP WHERE user_id = @ID
    ELSE
        INSERT INTO Tokens VALUES (@ID, @TOKEN, @EXP)
COMMIT TRAN
---GetGradeComponents---
--Selects everything needed to calcualte the student's grades
SELECT TOP 1
    E.auto_test_grade_value,
    E.style_test_grade_value,
    S.time_submitted,
    E.late_submission_settings,
    S.auto_grade,
    S.style_grade,
    S.manual_final_grade,
    S.submission_date_id,
    E.exercise_id
    FROM Submission as S
    INNER JOIN Exercise as E
        ON S.exercise_id = E.exercise_id
        AND S.submission_id = @ID
