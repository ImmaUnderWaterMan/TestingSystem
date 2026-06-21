using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;
using TestingSystem.Models.ViewModels;

namespace TestingSystem.Pages.Admin.Tests
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly TestRepository _testRepository;
        private readonly DisciplineRepository _disciplineRepository;

        public CreateModel(TestRepository testRepository, DisciplineRepository disciplineRepository)
        {
            _testRepository = testRepository;
            _disciplineRepository = disciplineRepository;
        }

        [BindProperty]
        public TestFormModel Input { get; set; } = new();
        public string DisciplineName { get; set; }

        public async Task OnGetAsync(int disciplineId)
        {
            Input.DisciplineId = disciplineId;
            var discipline = await _disciplineRepository.GetByIdAsync(disciplineId);
            DisciplineName = discipline?.Name;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var discipline = await _disciplineRepository.GetByIdAsync(Input.DisciplineId);
            DisciplineName = discipline?.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var test = new Test
            {
                DisciplineId = Input.DisciplineId,
                Title = Input.Title,
                Description = Input.Description,
                TimeLimitMinutes = Input.TimeLimitMinutes,
                PassingScore = Input.PassingScore,
                IsPublished = false,
                CreatedAt = DateTime.Now
            };

            var testId = await _testRepository.CreateAsync(test);

            TempData["Message"] = $"Тест '{test.Title}' создан! Теперь добавьте вопросы.";
            TempData["MessageType"] = "success";
            return RedirectToPage("Questions", new { testId });
        }
    }
}