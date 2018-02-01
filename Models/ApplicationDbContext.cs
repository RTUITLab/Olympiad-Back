using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Students { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<ExerciseData> TestData { get; set; }
    }
}
