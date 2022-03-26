using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Models.Checking;
using Models.Exercises;
using Models.Links;
using Models.Solutions;
using Models.UserModels;
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
            builder.Entity<UserToChallenge>()
                .HasKey(utc => new { utc.ChallengeId, utc.UserId });
            builder.Entity<UserToChallenge>()
                .HasOne(utc => utc.Challenge)
                .WithMany(c => c.UsersToChallenges)
                .HasForeignKey(utc => utc.ChallengeId);
            builder.Entity<UserToChallenge>()
                .HasOne(utc => utc.User)
                .WithMany(u => u.UsersToChallenges)
                .HasForeignKey(utc => utc.UserId);

            builder.Entity<User>()
                .HasIndex(u => u.StudentID)
                .IsUnique();

            builder.Entity<User>()
                .HasMany(u => u.Claims)
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            builder.Entity<ExerciseDataGroup>()
                .HasIndex(dg => new { dg.Title, dg.ExerciseId })
                .IsUnique();
        }
        public DbSet<User> Students { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseDataGroup> TestDataGroups { get; set; }
        public DbSet<ExerciseData> TestData { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<SolutionCheck> SolutionChecks { get; set; }
        public DbSet<SolutionBuildLog> SolutionBuildLogs { get; set; }
        public DbSet<LoginEvent> LoginEvents { get; set; }
    }
}
