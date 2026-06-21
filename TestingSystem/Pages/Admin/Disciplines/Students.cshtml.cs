using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Admin.Disciplines
{
    [Authorize(Roles = "Admin")]
    public class StudentsModel : PageModel
    {
        private readonly DisciplineRepository _disciplineRepository;

        public StudentsModel(DisciplineRepository disciplineRepository)
        {
            _disciplineRepository = disciplineRepository;
        }

        public Discipline Discipline { get; set; }
        public IEnumerable<User> AssignedStudents { get; set; } = new List<User>();
        public IEnumerable<User> AvailableStudents { get; set; } = new List<User>();
        public string Message { get; set; }
        public string MessageType { get; set; }

        public async Task OnGetAsync(int id)
        {
            Discipline = await _disciplineRepository.GetByIdAsync(id);
            if (Discipline == null)
            {
                RedirectToPage("Index");
                return;
            }

            AssignedStudents = await _disciplineRepository.GetStudentsAsync(id);

            var allStudents = await _disciplineRepository.GetAllStudentsAsync();
            var assignedIds = AssignedStudents.Select(s => s.Id).ToHashSet();
            AvailableStudents = allStudents.Where(s => !assignedIds.Contains(s.Id));

            if (TempData["Message"] != null)
            {
                Message = TempData["Message"].ToString();
                MessageType = TempData["MessageType"]?.ToString() ?? "info";
            }
        }

        public async Task<IActionResult> OnPostAddStudentAsync(int studentId, int disciplineId)
        {
            await _disciplineRepository.AssignStudentAsync(studentId, disciplineId);

            TempData["Message"] = "Студент успешно добавлен в дисциплину!";
            TempData["MessageType"] = "success";

            return RedirectToPage(new { id = disciplineId });
        }

        public async Task<IActionResult> OnPostRemoveStudentAsync(int studentId, int disciplineId)
        {
            await _disciplineRepository.RemoveStudentAsync(studentId, disciplineId);

            TempData["Message"] = "Студент удалён из дисциплины!";
            TempData["MessageType"] = "info";

            return RedirectToPage(new { id = disciplineId });
        }
    }
}