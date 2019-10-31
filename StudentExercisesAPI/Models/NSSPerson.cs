using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class NSSPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SlackHandle { get; set; }
        
        public int CohortId { get; set; }
        public Cohort Cohort { get; set; }
    }
}
