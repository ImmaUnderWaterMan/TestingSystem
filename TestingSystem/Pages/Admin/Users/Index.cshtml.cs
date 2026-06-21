using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestingSystem.Data.Repositories;

namespace TestingSystem.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserRepository _userRepository;

        public IndexModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<Models.User> Users { get; set; } = new List<Models.User>();
        public IEnumerable<Role> Roles { get; set; } = new List<Role>();
        public int? SelectedRoleId { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }

        public async Task OnGetAsync(int? roleId)
        {
            SelectedRoleId = roleId;
            Roles = await _userRepository.GetAllRolesAsync();

            if (roleId.HasValue)
            {
                Users = await _userRepository.GetByRoleAsync(roleId.Value);
            }
            else
            {
                Users = await _userRepository.GetAllAsync();
            }

            // Получаем сообщение из TempData
            if (TempData["Message"] != null)
            {
                Message = TempData["Message"].ToString();
                MessageType = TempData["MessageType"]?.ToString() ?? "info";
            }
        }
    }
}