using System.ComponentModel.DataAnnotations;

namespace TestingSystem.Models.ViewModels
{
    public class TestFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название теста")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 200 символов")]
        [Display(Name = "Название теста")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Описание не должно превышать 1000 символов")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Range(5, 180, ErrorMessage = "Время должно быть от 5 до 180 минут")]
        [Display(Name = "Время на выполнение (минут)")]
        public int? TimeLimitMinutes { get; set; }

        [Range(1, 100, ErrorMessage = "Проходной балл должен быть от 1 до 100")]
        [Display(Name = "Проходной балл (%)")]
        public int PassingScore { get; set; } = 50;

        [Display(Name = "Опубликован")]
        public bool IsPublished { get; set; }

        public int DisciplineId { get; set; }
    }
}