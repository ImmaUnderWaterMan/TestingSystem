using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Admin.Disciplines
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly DisciplineRepository _disciplineRepository;

        public DeleteModel(DisciplineRepository disciplineRepository)
        {
            _disciplineRepository = disciplineRepository;
        }

        public Discipline Discipline { get; set; }
        public int StudentsCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Discipline = await _disciplineRepository.GetByIdAsync(id);
            if (Discipline == null)
            {
                return NotFound();
            }

            var students = await _disciplineRepository.GetStudentsAsync(id);
            StudentsCount = students.Count();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var discipline = await _disciplineRepository.GetByIdAsync(id);
            if (discipline == null)
            {
                return NotFound();
            }

            await _disciplineRepository.DeleteAsync(id);

            TempData["Message"] = $"Дисциплина '{discipline.Name}' успешно удалена!";
            TempData["MessageType"] = "success";

            return RedirectToPage("Index");
        }
    }
}