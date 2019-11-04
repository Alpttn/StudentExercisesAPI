//this is the controller for Cohorts
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
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
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
        // GET: api/Cohort ***code for get all cohorts
        [HttpGet]
        public async Task<IActionResult> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT c.Id AS 'CohortId', c.Name, s.Id AS 'StudentId', s.FirstName AS 'StudentFirstName', s.LastName AS 'StudentLastName', s.SlackHandle AS 'StudentSlackHandle',
                                        i.Id AS 'InstructorId', i.FirstName, i.LastName, i.SlackHandle, i.Speciality
                                        FROM Cohort c
                                        LEFT JOIN Student s ON s.CohortId = c.Id    
                                        LEFT JOIN Instructor i ON i.CohortId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();
                    Dictionary<int, Student> students = new Dictionary<int, Student>();
                    Dictionary<int, Instructor> instructors = new Dictionary<int, Instructor>();

                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("CohortId")); //get the id
                        int studentId = reader.GetInt32(reader.GetOrdinal("StudentId")); //get the id
                        int instructorId = reader.GetInt32(reader.GetOrdinal("InstructorId")); //get the id

                        if (!cohorts.ContainsKey(cohortId)) //have I seen this student before?
                        {
                            Cohort newCohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")), //should I delete this one? Changed the AS
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StudentList = new List<Student>(),
                                InstructorList = new List<Instructor>()
                            };
                            cohorts.Add(cohortId, newCohort);
                        }

                        Cohort fromDictionary = cohorts[cohortId];
                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                        {
                            if (!students.ContainsKey(studentId))
                            {
                                Student newStudent = new Student()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")), //need to disambiguate
                                    LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle"))
                                };
                                students.Add(studentId, newStudent);
                                fromDictionary.StudentList.Add(newStudent);
                            }
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                        {
                            if (!instructors.ContainsKey(instructorId))
                            {
                                Instructor newInstructor = new Instructor()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")), //need to disambiguate
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    Speciality = reader.GetString(reader.GetOrdinal("Speciality"))
                                };
                                instructors.Add(instructorId, newInstructor);
                                fromDictionary.InstructorList.Add(newInstructor);
                            }
                        }
                    }
                    reader.Close();

                    /*
                        The Ok() method is an abstraction that constructs
                        a new HTTP response with a 200 status code, and converts
                        your IEnumerable into a JSON string to be sent back to
                        the requessting client application.
                    */
                    return Ok(cohorts.Values);


                }
            }
        }



        // GET: api/Cohort/5
        //[HttpGet("{id}", Name = "GetCohort")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Cohort
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Cohort/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}


