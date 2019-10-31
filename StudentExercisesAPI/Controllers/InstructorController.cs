//this controller is for instructors
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
    public class InstructorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InstructorController(IConfiguration config)
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
        // GET: api/Instructor get all instructors
        [HttpGet]
        public async Task<IActionResult> GetAllInstructors()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.FirstName, i.LastName, i.SlackHandle, i.Speciality, i.CohortId, c.Name AS CohortName
                        FROM Instructor i INNER JOIN Cohort c ON i.CohortId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Instructor> instructors = new Dictionary<int, Instructor>();

                    while (reader.Read())
                    {
                        int instructorId = reader.GetInt32(reader.GetOrdinal("Id")); //get the id
                        if (!instructors.ContainsKey(instructorId)) //have I seen this instructor before?
                        {

                            Instructor newInstructor = new Instructor
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                Speciality = reader.GetString(reader.GetOrdinal("Speciality")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                }
                            };
                            instructors.Add(instructorId, newInstructor);
                        }

                     

                        /*
                            The Ok() method is an abstraction that constructs
                            a new HTTP response with a 200 status code, and converts
                            your IEnumerable into a JSON string to be sent back to
                            the requessting client application.
                        */
                    }
                        reader.Close();
                        return Ok(instructors.Values);
                }
            }
        }

        // GET: api/Instructor/5 ***Get instructor by Id
        [HttpGet("{id}", Name = "GetInstructor")]
        public IActionResult GetInstructor(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.id, i.FirstName, i.LastName, i.SlackHandle, i.Speciality, i.CohortId, c.Name AS CohortName  
                                          FROM Instructor i LEFT JOIN Cohort c ON i.CohortId = c.Id
                                         WHERE i.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor anInstructor = null;
                    if (reader.Read())
                    {
                        anInstructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Speciality = reader.GetString(reader.GetOrdinal("Speciality")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                            }
                        };
                    }

                    reader.Close();
                    if (anInstructor == null)
                    {
                        return NotFound();
                    }

                    return Ok(anInstructor);
                }
            }
        }

        // POST: api/Instructor ***code for adding new Instructor
        [HttpPost]
        public void AddInstructor([FromBody] Instructor newInstructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Instructor (FirstName, LastName, SlackHandle, Speciality, CohortId)
                                        VALUES (@FirstName, @LastName, @SlackHandle, @Speciality, @CohortId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newInstructor.FirstName)); //lowercase
                    cmd.Parameters.Add(new SqlParameter("@lastName", newInstructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@specility", newInstructor.Speciality));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newInstructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", newInstructor.CohortId));
                    cmd.ExecuteNonQuery();
                }
            }
        }


        //PUT: api/Instructor/5
        [HttpPut("{id}")]
        public void UpdateInstructor([FromRoute] int id, [FromBody] Instructor newInstructor) //newMonster
        {
            //try
            //{
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Instructor
                                            SET FirstName = @firstName,
                                                LastName = @lastName,
                                                SlackHandle = @slackHandle,
                                                Speciality = @speciality,
                                                CohortId = @cohortId,
                                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newInstructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", newInstructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newInstructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@specialty", newInstructor.Speciality));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", newInstructor.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    int rowsAffected = cmd.ExecuteNonQuery();
                }
            }
        }

        //DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteInstructor([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Instructor
                                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    int rowsAffected = cmd.ExecuteNonQuery();

                }
            }

        }
    }
}
