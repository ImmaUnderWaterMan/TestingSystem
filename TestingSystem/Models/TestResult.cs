using System;

namespace TestingSystem.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int Score { get; set; }
        public int TotalPoints { get; set; }
        public int Percentage { get; set; }
        public bool IsPassed { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string StudentName { get; set; }
        public string TestTitle { get; set; }
        public string DisciplineName { get; set; }
    }
}