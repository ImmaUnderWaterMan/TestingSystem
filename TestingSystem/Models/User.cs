using System;

namespace TestingSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string FullName => $"{LastName} {FirstName}";
        public string RoleName { get; set; }
    }
}