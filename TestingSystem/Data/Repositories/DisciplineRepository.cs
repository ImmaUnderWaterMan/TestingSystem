using Dapper;
using TestingSystem.Models;

namespace TestingSystem.Data.Repositories
{
    public class DisciplineRepository
    {
        private readonly DatabaseHelper _db;

        public DisciplineRepository(DatabaseHelper db)
        {
            _db = db;
        }

        // Получить все дисциплины
        public async Task<IEnumerable<Discipline>> GetAllAsync()
        {
            var sql = @"
                SELECT d.*, u.FirstName + ' ' + u.LastName as TeacherName
                FROM Disciplines d
                LEFT JOIN Users u ON d.TeacherId = u.Id
                ORDER BY d.Name";

            return await _db.QueryAsync<Discipline>(sql);
        }

        // Получить дисциплину по ID
        public async Task<Discipline> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT d.*, u.FirstName + ' ' + u.LastName as TeacherName
                FROM Disciplines d
                LEFT JOIN Users u ON d.TeacherId = u.Id
                WHERE d.Id = @Id";

            return await _db.QueryFirstOrDefaultAsync<Discipline>(sql, new { Id = id });
        }

        // Создать дисциплину
        public async Task<int> CreateAsync(Discipline discipline)
        {
            var sql = @"
                INSERT INTO Disciplines (Name, Description, TeacherId, CreatedAt)
                VALUES (@Name, @Description, @TeacherId, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.ExecuteScalarAsync<int>(sql, discipline);
        }

        // Обновить дисциплину
        public async Task UpdateAsync(Discipline discipline)
        {
            var sql = @"
                UPDATE Disciplines 
                SET Name = @Name, 
                    Description = @Description, 
                    TeacherId = @TeacherId
                WHERE Id = @Id";

            await _db.ExecuteAsync(sql, discipline);
        }

        // Удалить дисциплину
        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Disciplines WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        // Получить дисциплины преподавателя
        public async Task<IEnumerable<Discipline>> GetByTeacherIdAsync(int teacherId)
        {
            var sql = @"
                SELECT d.*, u.FirstName + ' ' + u.LastName as TeacherName
                FROM Disciplines d
                LEFT JOIN Users u ON d.TeacherId = u.Id
                WHERE d.TeacherId = @TeacherId
                ORDER BY d.Name";

            return await _db.QueryAsync<Discipline>(sql, new { TeacherId = teacherId });
        }

        // Получить дисциплины студента
        public async Task<IEnumerable<Discipline>> GetByStudentIdAsync(int studentId)
        {
            var sql = @"
                SELECT d.*, u.FirstName + ' ' + u.LastName as TeacherName
                FROM Disciplines d
                INNER JOIN UserDisciplines ud ON d.Id = ud.DisciplineId
                LEFT JOIN Users u ON d.TeacherId = u.Id
                WHERE ud.UserId = @StudentId
                ORDER BY d.Name";

            return await _db.QueryAsync<Discipline>(sql, new { StudentId = studentId });
        }

        // Назначить студента на дисциплину
        public async Task AssignStudentAsync(int userId, int disciplineId)
        {
            var sql = @"
                IF NOT EXISTS (SELECT 1 FROM UserDisciplines WHERE UserId = @UserId AND DisciplineId = @DisciplineId)
                BEGIN
                    INSERT INTO UserDisciplines (UserId, DisciplineId, AssignedAt) 
                    VALUES (@UserId, @DisciplineId, GETDATE())
                END";

            await _db.ExecuteAsync(sql, new { UserId = userId, DisciplineId = disciplineId });
        }

        // Удалить студента из дисциплины
        public async Task RemoveStudentAsync(int userId, int disciplineId)
        {
            var sql = "DELETE FROM UserDisciplines WHERE UserId = @UserId AND DisciplineId = @DisciplineId";
            await _db.ExecuteAsync(sql, new { UserId = userId, DisciplineId = disciplineId });
        }

        // Получить студентов дисциплины
        public async Task<IEnumerable<User>> GetStudentsAsync(int disciplineId)
        {
            var sql = @"
                SELECT u.*, r.Name as RoleName
                FROM Users u
                INNER JOIN UserDisciplines ud ON u.Id = ud.UserId
                INNER JOIN Roles r ON u.RoleId = r.Id
                WHERE ud.DisciplineId = @DisciplineId AND u.RoleId = 3
                ORDER BY u.LastName, u.FirstName";

            return await _db.QueryAsync<User>(sql, new { DisciplineId = disciplineId });
        }

        // Получить всех преподавателей
        public async Task<IEnumerable<User>> GetTeachersAsync()
        {
            var sql = @"
                SELECT Id, FirstName, LastName, Login
                FROM Users 
                WHERE RoleId = 2
                ORDER BY LastName, FirstName";

            return await _db.QueryAsync<User>(sql);
        }

        // Получить всех студентов
        public async Task<IEnumerable<User>> GetAllStudentsAsync()
        {
            var sql = @"
                SELECT Id, FirstName, LastName, Login
                FROM Users 
                WHERE RoleId = 3
                ORDER BY LastName, FirstName";

            return await _db.QueryAsync<User>(sql);
        }
    }
}