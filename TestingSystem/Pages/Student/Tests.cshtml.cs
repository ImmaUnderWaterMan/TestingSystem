using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class TestsModel : PageModel
    {
        private readonly DisciplineRepository _disciplineRepository;
        private readonly TestRepository _testRepository;
        private readonly TestResultRepository _testResultRepository;

        public TestsModel(DisciplineRepository disciplineRepository,
                         TestRepository testRepository,
                         TestResultRepository testResultRepository)
        {
            _disciplineRepository = disciplineRepository;
            _testRepository = testRepository;
            _testResultRepository = testResultRepository;
        }

        public Discipline Discipline { get; set; }
        public IEnumerable<Test> Tests { get; set; } = new List<Test>();
        public Dictionary<int, int> QuestionsCount { get; set; } = new();
        public HashSet<int> PassedTests { get; set; } = new();
        public Dictionary<int, int> PassedResults { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int disciplineId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            Discipline = await _disciplineRepository.GetByIdAsync(disciplineId);
            if (Discipline == null)
            {
                return RedirectToPage("Index");
            }

            // Проверяем, что студент записан на эту дисциплину
            var studentDisciplines = await _disciplineRepository.GetByStudentIdAsync(userId);
            if (!studentDisciplines.Any(d => d.Id == disciplineId))
            {
                return RedirectToPage("Index");
            }

            // Получаем только опубликованные тесты
            var allTests = await _testRepository.GetByDisciplineIdAsync(disciplineId);
            Tests = allTests.Where(t => t.IsPublished);

            foreach (var test in Tests)
            {
                var questions = await _testRepository.GetQuestionsByTestIdAsync(test.Id);
                QuestionsCount[test.Id] = questions.Count();
            }

            return Page();
        }
    }
}