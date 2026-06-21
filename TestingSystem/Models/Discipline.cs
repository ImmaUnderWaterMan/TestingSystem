using System;

namespace TestingSystem.Models
{
    public class Discipline
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? TeacherId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TeacherName { get; set; }
    }
}