using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class TakeTestModel : PageModel
    {
        private readonly TestRepository _testRepository;
        private readonly TestResultRepository _testResultRepository;

        public TakeTestModel(TestRepository testRepository, TestResultRepository testResultRepository)
        {
            _testRepository = testRepository;
            _testResultRepository = testResultRepository;
        }

        public Test Test { get; set; }
        public List<Question> Questions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int testId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Проверяем, не проходил ли уже
            if (await _testResultRepository.HasPassedTestAsync(userId, testId))
            {
                TempData["Message"] = "Вы уже проходили этот тест!";
                TempData["MessageType"] = "warning";
                return RedirectToPage("Index");
            }

            Test = await _testRepository.GetByIdAsync(testId);
            if (Test == null || !Test.IsPublished)
            {
                return NotFound();
            }

            var questions = await _testRepository.GetQuestionsByTestIdAsync(testId);
            foreach (var q in questions)
            {
                if (q.QuestionType != 2) // Не текстовый
                {
                    q.Answers = (await _testRepository.GetAnswersByQuestionIdAsync(q.Id)).ToList();
                }
            }
            Questions = questions.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int testId,
            [FromForm] Dictionary<int, int[]> answers,
            [FromForm] Dictionary<int, string> textAnswers)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            Test = await _testRepository.GetByIdAsync(testId);
            var questions = await _testRepository.GetQuestionsByTestIdAsync(testId);

            int totalPoints = 0;
            int earnedPoints = 0;

            // Создаём запись о результате
            var result = new TestResult
            {
                UserId = userId,
                TestId = testId,
                Score = 0,
                TotalPoints = 0,
                Percentage = 0,
                IsPassed = false,
                StartedAt = DateTime.Now,
                CompletedAt = DateTime.Now
            };

            var resultId = await _testResultRepository.CreateResultAsync(result);

            // Проверяем каждый вопрос
            foreach (var question in questions)
            {
                totalPoints += question.Points;
                bool questionCorrect = false;

                if (question.QuestionType == 0) // Один ответ
                {
                    if (answers != null && answers.ContainsKey(question.Id))
                    {
                        var selectedAnswers = answers[question.Id];
                        if (selectedAnswers.Length > 0)
                        {
                            var selectedAnswerId = selectedAnswers[0];
                            var allAnswers = await _testRepository.GetAnswersByQuestionIdAsync(question.Id);
                            var correctAnswer = allAnswers.FirstOrDefault(a => a.IsCorrect);

                            questionCorrect = correctAnswer != null && correctAnswer.Id == selectedAnswerId;
                            if (questionCorrect)
                            {
                                earnedPoints += question.Points;
                            }

                            await _testResultRepository.CreateUserAnswerAsync(new UserAnswer
                            {
                                TestResultId = resultId,
                                QuestionId = question.Id,
                                SelectedAnswerId = selectedAnswerId,
                                IsCorrect = questionCorrect
                            });
                        }
                    }
                }
                else if (question.QuestionType == 1) // Несколько ответов
                {
                    if (answers != null && answers.ContainsKey(question.Id))
                    {
                        var selectedAnswers = answers[question.Id].ToHashSet();
                        var allAnswers = await _testRepository.GetAnswersByQuestionIdAsync(question.Id);
                        var correctAnswers = allAnswers.Where(a => a.IsCorrect).Select(a => a.Id).ToHashSet();

                        // Проверяем, что выбраны ВСЕ правильные ответы и НИ ОДНОГО неправильного
                        questionCorrect = selectedAnswers.SetEquals(correctAnswers);

                        if (questionCorrect)
                        {
                            earnedPoints += question.Points;
                        }

                        // Сохраняем все выбранные ответы
                        foreach (var selectedId in selectedAnswers)
                        {
                            await _testResultRepository.CreateUserAnswerAsync(new UserAnswer
                            {
                                TestResultId = resultId,
                                QuestionId = question.Id,
                                SelectedAnswerId = selectedId,
                                IsCorrect = correctAnswers.Contains(selectedId)
                            });
                        }
                    }
                }
                else // Текстовый ответ
                {
                    if (textAnswers != null && textAnswers.ContainsKey(question.Id))
                    {
                        var textAnswer = textAnswers[question.Id];

                        await _testResultRepository.CreateUserAnswerAsync(new UserAnswer
                        {
                            TestResultId = resultId,
                            QuestionId = question.Id,
                            TextAnswer = textAnswer,
                            IsCorrect = false // Текстовые ответы проверяются вручную
                        });

                        // Текстовые ответы пока не считаем
                        totalPoints -= question.Points;
                    }
                }
            }

            // Считаем процент
            int percentage = totalPoints > 0 ? (earnedPoints * 100) / totalPoints : 0;
            bool isPassed = percentage >= Test.PassingScore;

            // Обновляем результат
            await _testResultRepository.UpdateResultAsync(resultId, earnedPoints, totalPoints, percentage, isPassed);

            TempData["Message"] = $"Тест завершён! Результат: {percentage}%";
            TempData["MessageType"] = isPassed ? "success" : "warning";

            return RedirectToPage("Result", new { resultId });
        }
    }
}