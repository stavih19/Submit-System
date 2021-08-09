WITH D2 as
(SELECT 
    S.exercise_id AS exercise_id,
    D.submission_date AS submission_date,
    S.submission_status AS submission_status
FROM Submitters AS SS
INNER JOIN Submission AS S
ON SS.submission_id = S.submission_id
AND S.submission_status = 0
AND SS.user_id = '576888433'
INNER JOIN Submission_Dates as D
ON S.submission_date_id = D.submission_date_id)
SELECT C.course_id,
    C.course_name, 
    C.course_number,
    E.exercise_id,
    E.exercise_name,
    ISNULL(D2.submission_date, D.submission_date) AS ddd
FROM Student_Course AS SC
INNER JOIN Courses AS C
ON C.course_id = SC.course_id
AND SC.user_id = '576888433'
INNER JOIN Exercise AS E
ON E.course_id = C.course_id
INNER JOIN Submission_Dates AS D
ON D.exercise_id = E.exercise_id
AND D.group_number = 0
LEFT JOIN D2
ON D2.exercise_id = D.exercise_id
AND (S.submission_status = null OR S.submission_status = 0)