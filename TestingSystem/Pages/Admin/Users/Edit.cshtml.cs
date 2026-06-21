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
    public class EditModel : PageModel
    {
        private readonly UserRepository _userRepository;

        public EditModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public UserFormModel Input { get; set; } = new();

        public SelectList RolesSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Input = new UserFormModel
            {
                Id = user.Id,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = user.RoleId
            };

            var roles = await _userRepository.GetAllRolesAsync();
            RolesSelectList = new SelectList(roles, "Id", "Name", user.RoleId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var roles = await _userRepository.GetAllRolesAsync();
            RolesSelectList = new SelectList(roles, "Id", "Name", Input.RoleId);

            if (!ModelState.IsValid)
            {
                return Page();
            }


            if (await _userRepository.LoginExistsAsync(Input.Login, Input.Id))
            {
                ModelState.AddModelError("Input.Login", "Пользователь с таким логином уже существует");
                return Page();
            }


            var user = new User
            {
                Id = Input.Id,
                Login = Input.Login,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                RoleId = Input.RoleId
            };

            await _userRepository.UpdateAsync(user);

            if (!string.IsNullOrEmpty(Input.Password))
            {
                var passwordHash = PasswordHelper.HashPassword(Input.Password);
                await _userRepository.UpdatePasswordAsync(Input.Id, passwordHash);
            }

            TempData["Message"] = $"Пользователь '{user.Login}' успешно обновлён!";
            TempData["MessageType"] = "success";

            return RedirectToPage("Index");
        }
    }
}