﻿--get a specific instructor
--SELECT i.id, i.FirstName, i.LastName, i.SlackHandle, i.Speciality, i.CohortId  
--                                          FROM Instructor i LEFT JOIN Cohort c ON i.CohortId = c.Id
--                                         WHERE i.Id = 1