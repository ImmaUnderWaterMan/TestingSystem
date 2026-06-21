using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;
using TestingSystem.Models.ViewModels;

namespace TestingSystem.Pages.Admin.Tests
{
    [Authorize(Roles = "Admin")]
    public class QuestionsModel : PageModel
    {
        private readonly TestRepository _testRepository;

        public QuestionsModel(TestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public Test Test { get; set; }
        public List<Question> Questions { get; set; } = new();

        // ✅ Вернули свойство
        public QuestionFormModel NewQuestion { get; set; } = new();

        public string Message { get; set; }
        public string MessageType { get; set; }

        public async Task OnGetAsync(int testId)
        {
            await LoadDataAsync(testId);
        }

        private async Task LoadDataAsync(int testId)
        {
            Test = await _testRepository.GetByIdAsync(testId);
            if (Test == null)
            {
                RedirectToPage("/Admin/Disciplines/Index");
                return;
            }

            var questions = await _testRepository.GetQuestionsByTestIdAsync(testId);
            foreach (var q in questions)
            {
                q.Answers = (await _testRepository.GetAnswersByQuestionIdAsync(q.Id)).ToList();
            }
            Questions = questions.ToList();

            if (TempData["Message"] != null)
            {
                Message = TempData["Message"].ToString();
                MessageType = TempData["MessageType"]?.ToString() ?? "info";
            }
        }

        // Обработчик добавления вопроса
        public async Task<IActionResult> OnPostAddQuestionAsync(int testId, [FromForm] QuestionFormModel newQuestion)
        {
            if (newQuestion == null || string.IsNullOrWhiteSpace(newQuestion.Text))
            {
                TempData["Message"] = "Текст вопроса не может быть пустым!";
                TempData["MessageType"] = "danger";
                return RedirectToPage(new { testId });
            }

            var question = new Question
            {
                TestId = testId,
                Text = newQuestion.Text,
                QuestionType = newQuestion.QuestionType,
                Points = newQuestion.Points,
                OrderNumber = Questions.Count + 1
            };

            await _testRepository.CreateQuestionAsync(question);

            TempData["Message"] = "Вопрос успешно добавлен!";
            TempData["MessageType"] = "success";

            return RedirectToPage(new { testId });
        }

        // Обработчик удаления вопроса
        public async Task<IActionResult> OnPostDeleteQuestionAsync(int questionId, int testId)
        {
            await _testRepository.DeleteQuestionAsync(questionId);

            TempData["Message"] = "Вопрос удалён!";
            TempData["MessageType"] = "info";

            return RedirectToPage(new { testId });
        }

        // Обработчик добавления ответа
        public async Task<IActionResult> OnPostAddAnswerAsync(int questionId, int testId, string answerText, bool isCorrect)
        {
            if (string.IsNullOrWhiteSpace(answerText))
            {
                TempData["Message"] = "Текст ответа не может быть пустым!";
                TempData["MessageType"] = "danger";
                return RedirectToPage(new { testId });
            }

            var answer = new Answer
            {
                QuestionId = questionId,
                Text = answerText,
                IsCorrect = isCorrect,
                OrderNumber = 1
            };

            await _testRepository.CreateAnswerAsync(answer);

            TempData["Message"] = $"Ответ добавлен! {(isCorrect ? "✅ Правильный" : "")}";
            TempData["MessageType"] = "success";

            return RedirectToPage(new { testId });
        }

        // Обработчик удаления ответа
        public async Task<IActionResult> OnPostDeleteAnswerAsync(int answerId, int questionId, int testId)
        {
            await _testRepository.DeleteAnswerAsync(answerId);

            TempData["Message"] = "Ответ удалён!";
            TempData["MessageType"] = "info";

            return RedirectToPage(new { testId });
        }
    }
}