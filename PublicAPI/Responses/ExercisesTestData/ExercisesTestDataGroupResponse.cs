using System;
using System.Collections.Generic;
using System.Text;

namespace PublicAPI.Responses.ExercisesTestData
{
    public class ExercisesTestDataGroupResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }
        public bool IsPublic { get; set; }
        public List<ExerciseDataResponse> TestCases { get; set; }
    }
}
