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
    public class StudentExercisesAPIController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentExercisesAPIController(IConfiguration config)
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
        // GET: api/StudentExercisesAPI ***Code for getting a list of exercises
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT Id, Name, Language FROM Exercise";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> exercises = new List<Exercise>();

                    while (reader.Read())
                    {
                        Exercise exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        };
                        exercises.Add(exercise);
                    }
                    reader.Close();

                    /*
                        The Ok() method is an abstraction that constructs
                        a new HTTP response with a 200 status code, and converts
                        your IEnumerable into a JSON string to be sent back to
                        the requessting client application.
                    */
                    return Ok(exercises);
                }
            }
        }

        // GET: api/StudentExercisesAPI/5 ***Code for getting a single exercise
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, name, language,  
                                          FROM Exercise
                                         WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise anExercise = null;
                    if (reader.Read())
                    {
                        anExercise = new Exercise()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Language = reader.GetString(reader.GetOrdinal("language")),
                        };
                    }

                    reader.Close();
                    if (anExercise == null)
                    {
                        return NotFound();
                    }

                    return Ok(anExercise);
                }
            }
        }
        // POST: api/StudentExercisesAPI ***Code for creating an exercise
        [HttpPost]
        public void Post([FromBody] Exercise newExercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (name, language)
                                        VALUES (@name, @language)";
                    cmd.Parameters.Add(new SqlParameter("@name", newExercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@language", newExercise.Language));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // PUT: api/StudentExercisesAPI/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
