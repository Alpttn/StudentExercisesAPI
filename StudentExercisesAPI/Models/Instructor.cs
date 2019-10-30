using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;


namespace StudentExercisesAPI.Models
{
    public class Instructor : NSSPerson
    {
        public string InstructorSpeciality { get; set; }

        //public Instructor(string firstName, string lastName, string slackHandle, Cohort cohort, string instructorSpeciality)
        //{
        //    FirstName = firstName;
        //    LastName = lastName;
        //    SlackHandle = slackHandle;
        //    Cohort = cohort;
        //    InstructorSpeciality = instructorSpeciality;
        //}

        //public void assignExercise(Student student, Exercise exercise)
        //{
        //    student.exerciseList.Add(exercise);
        //}

    }
}
