--get a specific instructor
--SELECT i.id, i.FirstName, i.LastName, i.SlackHandle, i.Speciality, i.CohortId  
--                                          FROM Instructor i LEFT JOIN Cohort c ON i.CohortId = c.Id
--                                         WHERE i.Id = 1

--SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.Id, c.Name
--                                        FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id

--SELECT Id, Name FROM Cohort
--SELECT * FROM Cohort

--SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.Name AS CohortName, 
--		se.ExerciseId, e.Name AS ExerciseName, e.Language
--	FROM Student s INNER JOIN Cohort c ON s.CohortId =c.Id
--	LEFT JOIN StudentExercise se ON se.StudentId = s.id
--	LEFT JOIN Exercise e on se.exerciseId = e.Id

--SELECT i.Id, i.FirstName, i.LastName, i.SlackHandle, i.Speciality, i.CohortId, c.Name AS CohortName
--                        FROM Instructor i INNER JOIN Cohort c ON i.CohortId = c.Id

--SELECT c.Id, c.Name, s.Id AS 'StudentId', s.FirstName, s.LastName, s.SlackHandle,
--                                        i.Id AS 'InstructorId', i.FirstName, i.LastName, i.SlackHandle, i.Speciality
--                                        FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id    
--                                        LEFT JOIN Instructor i ON i.CohortId = c.Id

SELECT c.Id, c.Name, s.Id AS 'StudentId', s.FirstName, s.LastName, s.SlackHandle,
                                        i.Id AS 'InstructorId', i.FirstName, i.LastName, i.SlackHandle, i.Speciality
                                        FROM Cohort c
                                         LEFT JOIN Student s ON s.CohortId = c.Id    
                                        LEFT JOIN Instructor i ON i.CohortId = c.Id;

--SELECT * FROM Student