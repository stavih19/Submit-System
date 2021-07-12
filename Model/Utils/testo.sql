SELECT S.submission_id AS ID,
    S.auto_grade AS AutoGrade,
    S.style_grade AS StyleGrade,
    S.manual_final_grade AS ManualGrade,
    E.auto_test_grade_value AS GradeWeight,
    E.style_test_grade_value AS StyleWeight, 
    E.max_submitters AS MaxSubmitters,
    E.late_submittion_settings AS LateubmittionSettings,
    E.exercise_name AS ExName,
    D.submission_date AS [Date],
    S.time_submitted AS TimeSubmitted,
    E.multiple_submissions AS IsMultipleSubmissions,
    S.submission_status AS [State],
    (SELECT C.chat_id as ID, C.chat_status AS IsClosed, C.chat_type as [Type]
     FROM Chat AS C
     WHERE C.submission_id = S.submission_id
        AND C.chat_type = 0
     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) AS ExtensionChat,
    (SELECT C.chat_id as ID, C.chat_status AS IsClosed, C.chat_type as [Type]
     FROM Chat AS C
     WHERE C.submission_id = S.submission_id
        AND C.chat_type = 1
     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) AS AppealChat,
    (SELECT user_id
     FROM Submitters AS SS
        WHERE SS.submission_id = S.submission_id
     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) AS [Subm_itters]
FROM Submitters as SS
    INNER JOIN Submission as S
        ON S.submission_id = SS.submission_id AND SS.user_id = '111111111'
    INNER JOIN Exercise as E
        ON E.exercise_id = S.exercise_id AND E.exercise_id = '89111_2021_ex1'
    INNER JOIN Submission_Dates as D
        ON S.submission_date_id = D.submission_date_id
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
