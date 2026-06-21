using Dapper;
using TestingSystem.Models;

namespace TestingSystem.Data.Repositories
{
    public class TestResultRepository
    {
        private readonly DatabaseHelper _db;

        public TestResultRepository(DatabaseHelper db)
        {
            _db = db;
        }

        // Создать результат теста
        public async Task<int> CreateResultAsync(TestResult result)
        {
            var sql = @"
                INSERT INTO TestResults (UserId, TestId, Score, TotalPoints, Percentage, IsPassed, StartedAt, CompletedAt)
                VALUES (@UserId, @TestId, @Score, @TotalPoints, @Percentage, @IsPassed, @StartedAt, @CompletedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.ExecuteScalarAsync<int>(sql, result);
        }

        // Сохранить ответ студента
        public async Task<int> CreateUserAnswerAsync(UserAnswer userAnswer)
        {
            var sql = @"
                INSERT INTO UserAnswers (TestResultId, QuestionId, SelectedAnswerId, TextAnswer, IsCorrect)
                VALUES (@TestResultId, @QuestionId, @SelectedAnswerId, @TextAnswer, @IsCorrect);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.ExecuteScalarAsync<int>(sql, userAnswer);
        }

        // Получить результат теста по ID
        public async Task<TestResult> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT tr.*, u.FirstName + ' ' + u.LastName as StudentName, t.Title as TestTitle
                FROM TestResults tr
                INNER JOIN Users u ON tr.UserId = u.Id
                INNER JOIN Tests t ON tr.TestId = t.Id
                WHERE tr.Id = @Id";

            return await _db.QueryFirstOrDefaultAsync<TestResult>(sql, new { Id = id });
        }

        // Получить результаты студента
        public async Task<IEnumerable<TestResult>> GetByUserIdAsync(int userId)
        {
            var sql = @"
                SELECT tr.*, t.Title as TestTitle, d.Name as DisciplineName
                FROM TestResults tr
                INNER JOIN Tests t ON tr.TestId = t.Id
                INNER JOIN Disciplines d ON t.DisciplineId = d.Id
                WHERE tr.UserId = @UserId
                ORDER BY tr.CompletedAt DESC";

            return await _db.QueryAsync<TestResult>(sql, new { UserId = userId });
        }

        // Получить ответы студента по результату
        public async Task<IEnumerable<UserAnswer>> GetUserAnswersByResultIdAsync(int testResultId)
        {
            var sql = @"
                SELECT ua.*, q.Text as QuestionText, a.Text as SelectedAnswerText
                FROM UserAnswers ua
                INNER JOIN Questions q ON ua.QuestionId = q.Id
                LEFT JOIN Answers a ON ua.SelectedAnswerId = a.Id
                WHERE ua.TestResultId = @TestResultId";

            return await _db.QueryAsync<UserAnswer>(sql, new { TestResultId = testResultId });
        }

        // Проверить, проходил ли студент уже этот тест
        public async Task<bool> HasPassedTestAsync(int userId, int testId)
        {
            var sql = "SELECT COUNT(*) FROM TestResults WHERE UserId = @UserId AND TestId = @TestId";
            var count = await _db.ExecuteScalarAsync<int>(sql, new { UserId = userId, TestId = testId });
            return count > 0;
        }

        // Получить результаты теста (для преподавателя)
        public async Task<IEnumerable<TestResult>> GetByTestIdAsync(int testId)
        {
            var sql = @"
                SELECT tr.*, u.FirstName + ' ' + u.LastName as StudentName, t.Title as TestTitle
                FROM TestResults tr
                INNER JOIN Users u ON tr.UserId = u.Id
                INNER JOIN Tests t ON tr.TestId = t.Id
                WHERE tr.TestId = @TestId
                ORDER BY tr.Percentage DESC";

            return await _db.QueryAsync<TestResult>(sql, new { TestId = testId });
        }
        // Обновить результат теста
        public async Task UpdateResultAsync(int id, int score, int totalPoints, int percentage, bool isPassed)
        {
            var sql = @"
        UPDATE TestResults 
        SET Score = @Score, 
            TotalPoints = @TotalPoints, 
            Percentage = @Percentage, 
            IsPassed = @IsPassed,
            CompletedAt = GETDATE()
        WHERE Id = @Id";

            await _db.ExecuteAsync(sql, new
            {
                Id = id,
                Score = score,
                TotalPoints = totalPoints,
                Percentage = percentage,
                IsPassed = isPassed
            });
        }
    }
}