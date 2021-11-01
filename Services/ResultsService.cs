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
                .Select(e => new ExerciseView
                {
                    ExerciseID = e.ExerciseID,
                    ExerciseName = e.ExerciseName
                })
                .ToListAsync();

            var simpleData = await db
                    .Solutions
                    .Where(s => s.Exercise.ChallengeId == challengeId)
                    .GroupBy(m => new
                    {
                        m.UserId,
                        m.User.StudentID,
                        m.User.FirstName,
                        m.ExerciseId
                    })
                    .OrderBy(s => s.Key.StudentID)
                    .Select(g => new
                    {
                        UserId = g.Key.UserId,
                        StudentID = g.Key.StudentID,
                        FirstName = g.Key.FirstName,
                        ExerciseId = g.Key.ExerciseId,
                        Score = g.Max(s => s.TotalScore)
                    })
                    .ToListAsync();


            var userSolutions = simpleData
                .GroupBy(s => new { s.UserId, s.StudentID, s.FirstName })
                .Select(g => new UserSolution
                {
                    User = new UserView { Id = g.Key.UserId, StudentId = g.Key.StudentID, FirstName = g.Key.FirstName },
                    Scores = g.ToDictionary(g => g.ExerciseId, g => g.Score)
                })
                .ToList();

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
            public List<ExerciseView> Exercises { get; set; }
            public List<UserSolution> UserSolutions { get; set; }
        }
        public class ExerciseView
        {
            public Guid ExerciseID { get; set; }
            public string ExerciseName { get; set; }
        }

        public class UserView
        {
            public Guid Id { get; set; }
            public string StudentId { get; set; }
            public string FirstName { get; set; }
        }
        public class UserSolution
        {
            public UserView User { get; set; }
            public Dictionary<Guid, int?> Scores { get; set; }
        }
    }
}
