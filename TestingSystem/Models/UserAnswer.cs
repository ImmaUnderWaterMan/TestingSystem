namespace TestingSystem.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int TestResultId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedAnswerId { get; set; }
        public string TextAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public string QuestionText { get; set; }
        public string SelectedAnswerText { get; set; }
    }
}