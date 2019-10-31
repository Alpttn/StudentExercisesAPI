//this is the controller for students
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using StudentExercisesAPI.Models;
using Microsoft.AspNetCore.Http;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: api/Student ***Code for getting a list of students
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle,
                                        s.CohortId, c.Name AS CohortName, 
		                                se.ExerciseId, e.Name AS ExerciseName, e.Language
	                                FROM Student s INNER JOIN Cohort c ON s.CohortId = c.Id
	                                    LEFT JOIN StudentExercise se ON se.StudentId = s.id
	                                    LEFT JOIN Exercise e on se.exerciseId = e.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Student> students = new Dictionary<int, Student>();

                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(reader.GetOrdinal("Id")); //get the id
                        if (! students.ContainsKey(studentId)) //have I seen this student before?
                        {
                            Student newStudent = new Student() //() if it does not..then lets create it.
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                }
                            };
                            students.Add(studentId, newStudent);
                        }

                        Student fromDictionary = students[studentId];

                        if (! reader.IsDBNull(reader.GetOrdinal("ExerciseId"))) //only if i have an exercise id then it will run.
                        {
                            Exercise anExercise = new Exercise()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                Name = reader.GetString(reader.GetOrdinal("ExerciseName")), //need to disambiguate
                                Language = reader.GetString(reader.GetOrdinal("Language"))
                            };
                        fromDictionary.exerciseList.Add(anExercise); //move up into if
                        }
                         //got an error saying idk how to serialize a dictionary. 


                        /*
                            The Ok() method is an abstraction that constructs
                            a new HTTP response with a 200 status code, and converts
                            your IEnumerable into a JSON string to be sent back to
                            the requessting client application.
                        */
                    }
                        reader.Close();
                        return Ok(students.Values);
                }
            }
        }
            
        

        // GET: api/Student/5 ***Code for get student by Id
        [HttpGet("{id}", Name = "GetStudent")]
        public IActionResult GetStudent(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.Name AS CohortName  
                                          FROM Student s LEFT JOIN Cohort c ON s.CohortId = c.Id
                                         WHERE s.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student aStudent = null;
                    if (reader.Read())
                    {
                        aStudent = new Student()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                            }
                        };
                    }

                    reader.Close();
                    if (aStudent == null)
                    {
                        return NotFound();
                    }

                    return Ok(aStudent);
                }
            }
        }

        // POST: api/Student ***code to post new student
        [HttpPost]
        public void AddStudent([FromBody] Student newStudent)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId)
                                        VALUES (@FirstName, @LastName, @SlackHandle, @CohortId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newStudent.FirstName)); //lowercase
                    cmd.Parameters.Add(new SqlParameter("@lastName", newStudent.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newStudent.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", newStudent.CohortId));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // PUT: api/Student/5 ***code to update student
        [HttpPut("{id}")]
        public void UpdateStudent([FromRoute] int id, [FromBody] Student newStudent) //newMonster
        {
            //try
            //{
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Student
                                            SET FirstName = @firstName,
                                                LastName = @lastName,
                                                SlackHandle = @slackHandle,
                                                CohortId = @cohortId,
                                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newStudent.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", newStudent.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newStudent.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", newStudent.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    int rowsAffected = cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE: api/ApiWithActions/5 *** code to delete student
        [HttpDelete("{id}")]
        public void DeleteStudent([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Student
                                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    int rowsAffected = cmd.ExecuteNonQuery();

                }
            }

        }
    }
}
