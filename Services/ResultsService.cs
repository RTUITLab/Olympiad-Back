using Microsoft.EntityFrameworkCore;
using Models;
using Models.Exercises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Services
{
    public class ResultsService
    {
        private readonly ApplicationDbContext db;

        public ResultsService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<ChallengeResults> GetChallengeResults(Guid challengeId)
        {
            var challengeName = await db.Challenges
                .Where(c => c.Id == challengeId)
                .Select(c => c.Name)
                .SingleOrDefaultAsync();

            var exercises = await db.Exercises
                .OrderBy(e => e.ExerciseName)
                .Where(e => e.ChallengeId == challengeId)
                .ToListAsync();

            var simpleData = await db
                    .Solutions
                    .Where(s => s.Exercise.ChallengeId == challengeId)
                    .Include(s => s.User)
                    .GroupBy(m => new { m.UserId, m.User.StudentID, m.ExerciseId })
                    .Select(g => new
                    {
                        UserId = g.Key.UserId,
                        StudentId = g.Key.StudentID,
                        ExerciseId = g.Key.ExerciseId,
                        Score = g.Max(s => s.TotalScore)
                    })
                    .ToListAsync();
            var userSolutions = simpleData
                .GroupBy(s => new { s.UserId, s.StudentId })
                .Select(g => new UserSolution
                {
                    UserId = g.Key.UserId,
                    StudentId = g.Key.StudentId,
                    Scores = g.ToDictionary(g => g.ExerciseId, g => g.Score)
                })
                .OrderBy(g => g.StudentId)
                .ToList();
            var targetIds = userSolutions.Select(u => u.UserId).ToArray();
            var users = await db.Users.Where(u => targetIds.Contains(u.Id)).ToListAsync();
            foreach (var usersolution in userSolutions)
            {
                usersolution.User = users.Single(u => u.Id == usersolution.UserId);
            }
            return new ChallengeResults
            {
                ChallengeName = challengeName,
                UserSolutions = userSolutions,
                Exercises = exercises
            };
        }

        public class ChallengeResults
        {
            public string ChallengeName { get; set; }
            public List<Exercise> Exercises { get; set; }
            public List<UserSolution> UserSolutions { get; set; }
        }

        public class UserSolution
        {
            public Guid UserId { get; set; }
            public string StudentId { get; set; }
            public User User { get; set; }
            public Dictionary<Guid, int?> Scores { get; set; }
        }
    }
}
