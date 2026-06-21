using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;

namespace TestingSystem.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly UserRepository _userRepository;

        public DeleteModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public User UserToDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UserToDelete = await _userRepository.GetByIdAsync(id);
            if (UserToDelete == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }


            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            if (user.Id == currentUserId)
            {
                TempData["Message"] = "Нельзя удалить самого себя!";
                TempData["MessageType"] = "danger";
                return RedirectToPage("Index");
            }

            await _userRepository.DeleteAsync(id);

            TempData["Message"] = $"Пользователь '{user.Login}' успешно удалён!";
            TempData["MessageType"] = "success";

            return RedirectToPage("Index");
        }
    }
}