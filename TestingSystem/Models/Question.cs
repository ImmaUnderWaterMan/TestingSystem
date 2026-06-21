using System.Collections.Generic;

namespace TestingSystem.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string Text { get; set; }
        public int QuestionType { get; set; } 
        public int Points { get; set; }
        public int OrderNumber { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }
}