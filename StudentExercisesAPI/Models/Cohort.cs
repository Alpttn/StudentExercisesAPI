using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class Cohort
    {
        public int Id {get; set;}
        public string Name { get; set; }
        public List<Student> StudentList { get; set; } = new List<Student>();

        public List<Instructor> InstructorList { get; set; } = new List<Instructor>();

        //public Cohort(string name)
        //{
        //    Name = name;
        //}

    }
}
