using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class Student : NSSPerson
    {
        public int Id { get; set; }
        //list of student exercises
        public List<Exercise> exerciseList { get; set; } = new List<Exercise>();

        //constructor is a method to instantiate. There is no object until you instantiate it. 
        //public Student(string firstName, string lastName, string slackHandle, Cohort cohort)
        //{
        //    FirstName = firstName;
        //    LastName = lastName;
        //    SlackHandle = slackHandle;
        //    Cohort = cohort;
        //    exerciseList = new List<Exercise>(); //this will instantiate it
        //}
    }
}
