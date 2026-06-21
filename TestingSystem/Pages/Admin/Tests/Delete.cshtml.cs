using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Admin.Tests
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly TestRepository _testRepository;
        private readonly DisciplineRepository _disciplineRepository;

        public DeleteModel(TestRepository testRepository, DisciplineRepository disciplineRepository)
        {
            _testRepository = testRepository;
            _disciplineRepository = disciplineRepository;
        }

        public Test Test { get; set; }
        public string DisciplineName { get; set; }
        public int QuestionsCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Test = await _testRepository.GetByIdAsync(id);
            if (Test == null)
            {
                return NotFound();
            }

            var discipline = await _disciplineRepository.GetByIdAsync(Test.DisciplineId);
            DisciplineName = discipline?.Name;

            var questions = await _testRepository.GetQuestionsByTestIdAsync(id);
            QuestionsCount = questions.Count();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var test = await _testRepository.GetByIdAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            await _testRepository.DeleteAsync(id);

            TempData["Message"] = $"Тест '{test.Title}' успешно удалён!";
            TempData["MessageType"] = "success";

            return RedirectToPage("Index", new { disciplineId = test.DisciplineId });
        }
    }
}