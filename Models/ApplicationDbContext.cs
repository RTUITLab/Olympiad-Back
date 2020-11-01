using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Models.Checking;
using Models.Exercises;
using Models.Lessons;
using Models.Solutions;
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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasIndex(u => u.StudentID)
                .IsUnique();

            builder.Entity<UserToGroup>(utg =>
            {
                utg.HasKey(u => new { u.GroupId, u.UserId });
                utg.HasOne(u => u.User)
                    .WithMany(u => u.UserToGroups)
                    .HasForeignKey(u => u.UserId);
                utg.HasOne(u => u.Group)
                    .WithMany(u => u.UserToGroups)
                    .HasForeignKey(u => u.GroupId);
            });
            builder.Entity<GroupToCourse>(utg =>
            {
                utg.HasKey(u => new { u.GroupId, u.CourseId });
                utg.HasOne(u => u.Course)
                    .WithMany(u => u.GroupToCourses)
                    .HasForeignKey(u => u.CourseId);
                utg.HasOne(u => u.Group)
                    .WithMany(u => u.GroupToCourses)
                    .HasForeignKey(u => u.GroupId);
            });
        }
        public DbSet<User> Students { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<ExerciseData> TestData { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<SolutionCheck> SolutionChecks { get; set; }
        public DbSet<SolutionBuildLog> SolutionBuildLogs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}
