using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;
using TestingSystem.Models.ViewModels;

namespace TestingSystem.Pages.Admin.Tests
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly TestRepository _testRepository;
        private readonly DisciplineRepository _disciplineRepository;

        public EditModel(TestRepository testRepository, DisciplineRepository disciplineRepository)
        {
            _testRepository = testRepository;
            _disciplineRepository = disciplineRepository;
        }

        [BindProperty]
        public TestFormModel Input { get; set; } = new();

        public string DisciplineName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var test = await _testRepository.GetByIdAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            Input = new TestFormModel
            {
                Id = test.Id,
                DisciplineId = test.DisciplineId,
                Title = test.Title,
                Description = test.Description,
                TimeLimitMinutes = test.TimeLimitMinutes,
                PassingScore = test.PassingScore,
                IsPublished = test.IsPublished
            };

            var discipline = await _disciplineRepository.GetByIdAsync(test.DisciplineId);
            DisciplineName = discipline?.Name;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var test = await _testRepository.GetByIdAsync(Input.Id);
                if (test != null)
                {
                    var discipline = await _disciplineRepository.GetByIdAsync(test.DisciplineId);
                    DisciplineName = discipline?.Name;
                }
                return Page();
            }

            var updatedTest = new Test
            {
                Id = Input.Id,
                DisciplineId = Input.DisciplineId,
                Title = Input.Title,
                Description = Input.Description,
                TimeLimitMinutes = Input.TimeLimitMinutes,
                PassingScore = Input.PassingScore,
                IsPublished = Input.IsPublished
            };

            await _testRepository.UpdateAsync(updatedTest);

            TempData["Message"] = $"Тест '{updatedTest.Title}' успешно обновлён!";
            TempData["MessageType"] = "success";

            return RedirectToPage("Index", new { disciplineId = Input.DisciplineId });
        }
    }
}