using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestingSystem.Data.Repositories;
using TestingSystem.Helpers;
using TestingSystem.Models;
using TestingSystem.Models.ViewModels;

namespace TestingSystem.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserRepository _userRepository;

        public CreateModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public UserFormModel Input { get; set; } = new();

        public SelectList RolesSelectList { get; set; }

        public async Task OnGetAsync()
        {
            var roles = await _userRepository.GetAllRolesAsync();
            RolesSelectList = new SelectList(roles, "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var roles = await _userRepository.GetAllRolesAsync();
            RolesSelectList = new SelectList(roles, "Id", "Name");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверяем уникальность логина
            if (await _userRepository.LoginExistsAsync(Input.Login))
            {
                ModelState.AddModelError("Input.Login", "Пользователь с таким логином уже существует");
                return Page();
            }

            // Хешируем пароль
            var passwordHash = PasswordHelper.HashPassword(Input.Password);

            var user = new User
            {
                Login = Input.Login,
                PasswordHash = passwordHash,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                RoleId = Input.RoleId,
                CreatedAt = DateTime.Now
            };

            await _userRepository.CreateAsync(user);

            TempData["Message"] = $"Пользователь '{user.Login}' успешно создан!";
            TempData["MessageType"] = "success";

            return RedirectToPage("Index");
        }
    }
}