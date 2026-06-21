using System.ComponentModel.DataAnnotations;

namespace TestingSystem.Models.ViewModels
{
    public class QuestionFormModel
    {
        [Required(ErrorMessage = "Введите текст вопроса")]
        [Display(Name = "Текст вопроса")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Выберите тип вопроса")]
        [Display(Name = "Тип вопроса")]
        public int QuestionType { get; set; } // 0 = один ответ, 1 = несколько, 2 = текст

        [Range(1, 100, ErrorMessage = "Баллы должны быть от 1 до 100")]
        [Display(Name = "Баллы за вопрос")]
        public int Points { get; set; } = 1;
    }

    public class AnswerFormModel
    {
        [Required(ErrorMessage = "Введите текст ответа")]
        [Display(Name = "Текст ответа")]
        public string Text { get; set; }

        [Display(Name = "Правильный ответ")]
        public bool IsCorrect { get; set; }
    }
}