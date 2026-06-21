using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Admin.Tests
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly TestRepository _testRepository;
        private readonly DisciplineRepository _disciplineRepository;

        public IndexModel(TestRepository testRepository, DisciplineRepository disciplineRepository)
        {
            _testRepository = testRepository;
            _disciplineRepository = disciplineRepository;
        }

        public Discipline Discipline { get; set; }
        public IEnumerable<Test> Tests { get; set; } = new List<Test>();
        public Dictionary<int, int> QuestionsCount { get; set; } = new();
        public int DisciplineId { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }

        public async Task OnGetAsync(int disciplineId)
        {
            DisciplineId = disciplineId;
            Discipline = await _disciplineRepository.GetByIdAsync(disciplineId);

            if (Discipline == null)
            {
                RedirectToPage("/Admin/Disciplines/Index");
                return;
            }

            Tests = await _testRepository.GetByDisciplineIdAsync(disciplineId);

            foreach (var test in Tests)
            {
                var questions = await _testRepository.GetQuestionsByTestIdAsync(test.Id);
                QuestionsCount[test.Id] = questions.Count();
            }

            if (TempData["Message"] != null)
            {
                Message = TempData["Message"].ToString();
                MessageType = TempData["MessageType"]?.ToString() ?? "info";
            }
        }

        public async Task<IActionResult> OnPostTogglePublishAsync(int testId)
        {
            await _testRepository.TogglePublishAsync(testId);

            TempData["Message"] = "Статус публикации изменён!";
            TempData["MessageType"] = "success";

            return RedirectToPage(new { disciplineId = DisciplineId });
        }
    }
}