using System.ComponentModel.DataAnnotations;

namespace TestingSystem.Models.ViewModels
{
    public class DisciplineFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название дисциплины")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 100 символов")]
        [Display(Name = "Название дисциплины")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Преподаватель")]
        public int? TeacherId { get; set; }

        public List<User> AvailableTeachers { get; set; } = new();
    }
}