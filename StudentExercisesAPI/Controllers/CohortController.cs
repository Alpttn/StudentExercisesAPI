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

                    cmd.CommandText = @"SELECT c.Id, c.Name, s.Id AS 'StudentId', s.FirstName, s.LastName, s.SlackHandle,
                                        i.Id AS 'InstructorId', i.FirstName, i.LastName, i.SlackHandle, i.Speciality
                                        FROM Cohort c
                                         LEFT JOIN Student s ON s.CohortId = c.Id    
                                        LEFT JOIN Instructor i ON i.CohortId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader(); 

                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();

                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("Id")); //get the id
                        if (!cohorts.ContainsKey(cohortId)) //have I seen this student before?
                        {
                            Cohort newCohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StudentList = new List<Student>(),
                                InstructorList = new List<Instructor>()

                            };
                            cohorts.Add(cohortId, newCohort);
                        }

                        Cohort fromDictionary = cohorts[cohortId];
                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                        {
                            Student newStudent = new Student()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")), //need to disambiguate
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
                            };
                            fromDictionary.StudentList.Add(newStudent);
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                        {
                            Instructor newInstructor = new Instructor()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")), //need to disambiguate
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                Speciality = reader.GetString(reader.GetOrdinal("Speciality"))
                            };
                            fromDictionary.InstructorList.Add(newInstructor);
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


