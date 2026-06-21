using System;

namespace TestingSystem.Models
{
    public class UserDiscipline
    {
        public int UserId { get; set; }
        public int DisciplineId { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}