-- Uses 3 consecutive hyphens to separate the page into keys and queries
---StudentDates---
SELECT C.course_id,
    C.course_name, 
    C.course_number,
    E.exercise_id,
    E.exercise_name,
    D.submission_date
FROM Submitters as SS
    INNER JOIN Submission as S 
        ON S.submission_id = SS.submission_id AND S.submission_status = 0 AND SS.user_id = @ID
    INNER JOIN Exercise as E
        ON S.exercise_id = E.exercise_id
    INNER JOIN Submission_Dates as D
        ON S.submission_date_id = D.submission_date_id
    INNER JOIN Courses as C
        ON C.course_id = E.course_id;
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
    D.submission_date,
    S.time_submitted,
    E.late_submittion_settings,
    S.auto_grade,
    S.style_grade,
    S.manual_final_grade
FROM Submitters as SS
    INNER JOIN Submission as S
        ON S.submission_id = SS.submission_id AND SS.user_id = @ID AND S.submission_status > 1
    INNER JOIN Submission_Dates as D
        ON S.submission_date_id = D.submission_date_id
    INNER JOIN Exercise as E
        ON S.exercise_id = E.exercise_id
    INNER JOIN Courses as C
        ON C.course_id = E.course_id
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
    S.submission_id, SS.user_id, U.name  
FROM Submissions AS S
    INNER JOIN Submittters as SS
        ON SS.submission_id = S.submission_id
    INNER JOIN Users as U
        ON UC.user_id = U.user_id
WHERE S.exercise_id = @EID
---aaa---
SELECT submission_date, reduction, [group]
FROM Submission_Dates
WHERE submission_id = @ID OR submission_id = null
ORDER BY submisison_date, reduction
---HasExercise---
SELECT 1
FROM @Table AS UC
    INNER JOIN Courses as C 
        ON UC.course_id = C.course_id
        AND UC.user_id = @UID
    INNER JOIN Exercise AS E
        ON E.course_id = C.course_id
        AND E.exercise_id = @RID
---HasSubmissionTeacher---
SELECT 1
FROM Submission as S
    INNER JOIN Exercise AS E
       ON E.exercuse_id = S.exercise_id
        AND S.submission_id = @RID
    INNER JOIN Courses as C 
        ON E.course_id = C.course_id 
    INNER JOIN Metargel_Course AS UC
        ON UC.course_id = C.course_id
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
    INNER JOIN Courses as C 
        ON E.course_id = C.course_id 
    INNER JOIN Metargel_Course AS UC
        ON UC.course_id = C.course_id
        AND UC.user_id = @UID
---HasChatStudent---
SELECT 1 
FROM Chat AS C
    INNER JOIN Submission AS S
        ON S.submission_id = C.submission_id
        AND C.chat_id = @RID
    INNER JOIN Submitters as SS
        ON SS.submission_id = S.submission_id
        AND SS.user_id = @UID
---HasDate---
SELECT 1
FROM Submisison_Dates as D
    INNER JOIN Exercise AS E
       ON E.exercuse_id = D.exercise_id
       AND D.submission_date_id = @RID
    INNER JOIN Courses as C 
        ON E.course_id = C.course_id 
    INNER JOIN Metargel_Course AS MC
        ON UC.course_id = C.course_id
        AND UC.user_id = @UID
---HasTest---
SELECT 1
FROM Tests as T
    INNER JOIN Exercise AS E
       ON E.exercuse_id = T.exercise_id
       AND T.test_id = @RID
    INNER JOIN Courses as C 
        ON E.course_id = C.course_id 
    INNER JOIN Metargel_Course AS MC
        ON UC.course_id = C.course_id
        AND UC.user_id = @UID
---GetRequests---
WITH FM (id) AS (SELECT MIN(message_id) FROM [Message] GROUP BY chat_id)
SELECT C.chat_id, M.sender_user_id, M.sender_name, M.message_value
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
SELECT C.course_name, C.course_number, C.course_id, E.exercise_id, E.exercise_name
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

    
