using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class ResultModel : PageModel
    {
        private readonly TestResultRepository _testResultRepository;
        private readonly TestRepository _testRepository;

        public ResultModel(TestResultRepository testResultRepository, TestRepository testRepository)
        {
            _testResultRepository = testResultRepository;
            _testRepository = testRepository;
        }

        public TestResult Result { get; set; }
        public Test Test { get; set; }
        public IEnumerable<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

        public async Task<IActionResult> OnGetAsync(int resultId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            Result = await _testResultRepository.GetByIdAsync(resultId);
            if (Result == null || Result.UserId != userId)
            {
                return NotFound();
            }

            Test = await _testRepository.GetByIdAsync(Result.TestId);
            if (Test == null)
            {
                return NotFound();
            }

            UserAnswers = await _testResultRepository.GetUserAnswersByResultIdAsync(resultId);

            return Page();
        }
    }
}