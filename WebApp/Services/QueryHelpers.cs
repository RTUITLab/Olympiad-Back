using Models;
using Models.Exercises;
using Olympiad.Shared.Models;
using PublicAPI.Requests.Account;
using PublicAPI.Responses.Challenges;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Services
{
    public static class QueryHelpers
    {
        public static IQueryable<User> FindByMatch(this IQueryable<User> users, string match)
        {
            if (string.IsNullOrWhiteSpace(match))
            {
                return users;
            }
            var words = (match ?? "").ToUpper().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            return words.Aggregate(users, (usersCollection, matcher) => usersCollection.Where(
                u =>
                    u.FirstName.ToUpper().Contains(matcher) ||
                    u.Email.ToUpper().Contains(matcher) ||
                    u.StudentID.ToUpper().Contains(matcher)));
        }
        public static IQueryable<User> FindByClaims(this IQueryable<User> users, IEnumerable<ClaimRequest> claims)
        {
            if (claims?.Any() == true)
            {
                foreach (var claim in claims)
                {
                    users = users.Where(u => u.Claims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value));
                }
            }
            return users;
        }

        public static IQueryable<Challenge> AvailableChallenges(this IQueryable<Challenge> challenges, Guid userId)
        {
            return challenges.Where(c =>
                c.ChallengeAccessType == ChallengeAccessType.Public ||
                c.UsersToChallenges.Any(utc => utc.UserId == userId)
            );
        }
    }
}
