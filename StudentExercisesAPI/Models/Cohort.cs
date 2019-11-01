using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesAPI.Models
{
    public class Cohort
    {
        public int Id {get; set;}
        [Required]
        [StringLength(11, MinimumLength=5)]
        public string Name { get; set; }
        public List<Student> StudentList { get; set; } = new List<Student>();

        public List<Instructor> InstructorList { get; set; } = new List<Instructor>();

        //public Cohort(string name)
        //{
        //    Name = name;
        //}

    }
}
