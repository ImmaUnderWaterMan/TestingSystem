using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;
using TestingSystem.Models.ViewModels;

namespace TestingSystem.Pages.Admin.Disciplines
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly DisciplineRepository _disciplineRepository;
        private readonly UserRepository _userRepository;

        public CreateModel(DisciplineRepository disciplineRepository, UserRepository userRepository)
        {
            _disciplineRepository = disciplineRepository;
            _userRepository = userRepository;
        }

        [BindProperty]
        public DisciplineFormModel Input { get; set; } = new();

        public SelectList TeachersSelectList { get; set; }

        public async Task OnGetAsync()
        {
            var teachers = await _disciplineRepository.GetTeachersAsync();
            TeachersSelectList = new SelectList(teachers, "Id", "FullName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var teachers = await _disciplineRepository.GetTeachersAsync();
            TeachersSelectList = new SelectList(teachers, "Id", "FullName");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var discipline = new Discipline
            {
                Name = Input.Name,
                Description = Input.Description,
                TeacherId = Input.TeacherId,
                CreatedAt = DateTime.Now
            };

            await _disciplineRepository.CreateAsync(discipline);

            TempData["Message"] = $"Дисциплина '{discipline.Name}' успешно создана!";
            TempData["MessageType"] = "success";

            return RedirectToPage("Index");
        }
    }
}