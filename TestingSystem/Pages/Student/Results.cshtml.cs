using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class ResultsModel : PageModel
    {
        private readonly TestResultRepository _testResultRepository;

        public ResultsModel(TestResultRepository testResultRepository)
        {
            _testResultRepository = testResultRepository;
        }

        public IEnumerable<TestResult> Results { get; set; } = new List<TestResult>();
        public string Message { get; set; }
        public string MessageType { get; set; }

        public async Task OnGetAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Results = await _testResultRepository.GetByUserIdAsync(userId);

            if (TempData["Message"] != null)
            {
                Message = TempData["Message"].ToString();
                MessageType = TempData["MessageType"]?.ToString() ?? "info";
            }
        }
    }
}