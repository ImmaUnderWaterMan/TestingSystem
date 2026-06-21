using Dapper;
using TestingSystem.Models;

namespace TestingSystem.Data.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseHelper _db;

        public UserRepository(DatabaseHelper db)
        {
            _db = db;
        }

        // Получить пользователя по логину (для входа)
        public async Task<User> GetByLoginAsync(string login)
        {
            var sql = @"
                SELECT u.*, r.Name as RoleName 
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.Id
                WHERE u.Login = @Login";

            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Login = login });
        }

        // Получить пользователя по ID
        public async Task<User> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT u.*, r.Name as RoleName 
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.Id
                WHERE u.Id = @Id";

            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        // Получить всех пользователей
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = @"
                SELECT u.*, r.Name as RoleName 
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.Id
                ORDER BY u.LastName, u.FirstName";

            return await _db.QueryAsync<User>(sql);
        }

        // Получить пользователей по роли
        public async Task<IEnumerable<User>> GetByRoleAsync(int roleId)
        {
            var sql = @"
                SELECT u.*, r.Name as RoleName 
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.Id
                WHERE u.RoleId = @RoleId
                ORDER BY u.LastName, u.FirstName";

            return await _db.QueryAsync<User>(sql, new { RoleId = roleId });
        }

        // Получить все роли
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            var sql = "SELECT * FROM Roles ORDER BY Id";
            return await _db.QueryAsync<Role>(sql);
        }

        // Создать нового пользователя
        public async Task<int> CreateAsync(User user)
        {
            var sql = @"
                INSERT INTO Users (Login, PasswordHash, FirstName, LastName, Email, RoleId)
                VALUES (@Login, @PasswordHash, @FirstName, @LastName, @Email, @RoleId);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.ExecuteScalarAsync(sql, user);
        }

        // Обновить пользователя (без пароля)
        public async Task UpdateAsync(User user)
        {
            var sql = @"
                UPDATE Users 
                SET FirstName = @FirstName, 
                    LastName = @LastName, 
                    Email = @Email, 
                    RoleId = @RoleId
                WHERE Id = @Id";

            await _db.ExecuteAsync(sql, user);
        }

        // Обновить пароль пользователя
        public async Task UpdatePasswordAsync(int userId, string passwordHash)
        {
            var sql = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = userId, PasswordHash = passwordHash });
        }

        // Удалить пользователя
        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Users WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        // Проверить, существует ли логин
        public async Task<bool> LoginExistsAsync(string login, int? excludeUserId = null)
        {
            var sql = "SELECT COUNT(*) FROM Users WHERE Login = @Login" +
                      (excludeUserId.HasValue ? " AND Id != @ExcludeUserId" : "");

            var count = await _db.ExecuteScalarAsync<int>(sql,
                new { Login = login, ExcludeUserId = excludeUserId });

            return count > 0;
        }
    }

    // Вспомогательный класс для ролей
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}