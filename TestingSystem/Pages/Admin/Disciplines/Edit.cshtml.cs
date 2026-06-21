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
    public class EditModel : PageModel
    {
        private readonly DisciplineRepository _disciplineRepository;

        public EditModel(DisciplineRepository disciplineRepository)
        {
            _disciplineRepository = disciplineRepository;
        }

        [BindProperty]
        public DisciplineFormModel Input { get; set; } = new();

        public SelectList TeachersSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var discipline = await _disciplineRepository.GetByIdAsync(id);
            if (discipline == null)
            {
                return NotFound();
            }

            Input = new DisciplineFormModel
            {
                Id = discipline.Id,
                Name = discipline.Name,
                Description = discipline.Description,
                TeacherId = discipline.TeacherId
            };

            var teachers = await _disciplineRepository.GetTeachersAsync();
            TeachersSelectList = new SelectList(teachers, "Id", "FullName", discipline.TeacherId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var teachers = await _disciplineRepository.GetTeachersAsync();
            TeachersSelectList = new SelectList(teachers, "Id", "FullName", Input.TeacherId);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var discipline = new Discipline
            {
                Id = Input.Id,
                Name = Input.Name,
                Description = Input.Description,
                TeacherId = Input.TeacherId
            };

            await _disciplineRepository.UpdateAsync(discipline);

            TempData["Message"] = $"Дисциплина '{discipline.Name}' успешно обновлена!";
            TempData["MessageType"] = "success";

            return RedirectToPage("Index");
        }
    }
}