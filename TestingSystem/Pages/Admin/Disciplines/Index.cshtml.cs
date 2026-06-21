using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Admin.Disciplines
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly DisciplineRepository _disciplineRepository;
        private readonly UserRepository _userRepository;

        public IndexModel(DisciplineRepository disciplineRepository, UserRepository userRepository)
        {
            _disciplineRepository = disciplineRepository;
            _userRepository = userRepository;
        }

        public IEnumerable<Discipline> Disciplines { get; set; } = new List<Discipline>();
        public Dictionary<int, int> StudentsCount { get; set; } = new();
        public string Message { get; set; }
        public string MessageType { get; set; }

        public async Task OnGetAsync()
        {
            Disciplines = await _disciplineRepository.GetAllAsync();

            // Получаем количество студентов для каждой дисциплины
            foreach (var discipline in Disciplines)
            {
                var students = await _disciplineRepository.GetStudentsAsync(discipline.Id);
                StudentsCount[discipline.Id] = students.Count();
            }

            if (TempData["Message"] != null)
            {
                Message = TempData["Message"].ToString();
                MessageType = TempData["MessageType"]?.ToString() ?? "info";
            }
        }
    }
}