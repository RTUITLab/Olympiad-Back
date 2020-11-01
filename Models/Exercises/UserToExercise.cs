using System;

namespace Models.Exercises
{
    public class UserToExercise
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
    }
}