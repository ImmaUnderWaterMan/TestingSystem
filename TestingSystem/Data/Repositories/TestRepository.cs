using Dapper;
using TestingSystem.Models;

namespace TestingSystem.Data.Repositories
{
    public class TestRepository
    {
        private readonly DatabaseHelper _db;

        public TestRepository(DatabaseHelper db)
        {
            _db = db;
        }

        // Получить все тесты дисциплины
        public async Task<IEnumerable<Test>> GetByDisciplineIdAsync(int disciplineId)
        {
            var sql = @"
                SELECT t.*, d.Name as DisciplineName
                FROM Tests t
                INNER JOIN Disciplines d ON t.DisciplineId = d.Id
                WHERE t.DisciplineId = @DisciplineId
                ORDER BY t.Title";

            return await _db.QueryAsync<Test>(sql, new { DisciplineId = disciplineId });
        }

        // Получить тест по ID
        public async Task<Test> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT t.*, d.Name as DisciplineName
                FROM Tests t
                INNER JOIN Disciplines d ON t.DisciplineId = d.Id
                WHERE t.Id = @Id";

            return await _db.QueryFirstOrDefaultAsync<Test>(sql, new { Id = id });
        }

        // Создать тест
        public async Task<int> CreateAsync(Test test)
        {
            var sql = @"
                INSERT INTO Tests (DisciplineId, Title, Description, TimeLimitMinutes, PassingScore, IsPublished, CreatedAt)
                VALUES (@DisciplineId, @Title, @Description, @TimeLimitMinutes, @PassingScore, @IsPublished, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.ExecuteScalarAsync<int>(sql, test);
        }

        // Обновить тест
        public async Task UpdateAsync(Test test)
        {
            var sql = @"
                UPDATE Tests 
                SET Title = @Title, 
                    Description = @Description, 
                    TimeLimitMinutes = @TimeLimitMinutes, 
                    PassingScore = @PassingScore,
                    IsPublished = @IsPublished
                WHERE Id = @Id";

            await _db.ExecuteAsync(sql, test);
        }

        // Удалить тест
        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Tests WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        // Опубликовать/скрыть тест
        public async Task TogglePublishAsync(int id)
        {
            var sql = "UPDATE Tests SET IsPublished = ~IsPublished WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        // Получить вопросы теста
        public async Task<IEnumerable<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            var sql = @"
                SELECT * FROM Questions 
                WHERE TestId = @TestId 
                ORDER BY OrderNumber";

            return await _db.QueryAsync<Question>(sql, new { TestId = testId });
        }

        // Получить вопрос по ID
        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            var sql = "SELECT * FROM Questions WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<Question>(sql, new { Id = id });
        }

        // Создать вопрос
        public async Task<int> CreateQuestionAsync(Question question)
        {
            var sql = @"
                INSERT INTO Questions (TestId, Text, QuestionType, Points, OrderNumber)
                VALUES (@TestId, @Text, @QuestionType, @Points, @OrderNumber);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.ExecuteScalarAsync<int>(sql, question);
        }

        // Обновить вопрос
        public async Task UpdateQuestionAsync(Question question)
        {
            var sql = @"
                UPDATE Questions 
                SET Text = @Text, 
                    QuestionType = @QuestionType, 
                    Points = @Points, 
                    OrderNumber = @OrderNumber
                WHERE Id = @Id";

            await _db.ExecuteAsync(sql, question);
        }

        // Удалить вопрос
        public async Task DeleteQuestionAsync(int id)
        {
            var sql = "DELETE FROM Questions WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        // Получить ответы вопроса
        public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            var sql = @"
                SELECT * FROM Answers 
                WHERE QuestionId = @QuestionId 
                ORDER BY OrderNumber";

            return await _db.QueryAsync<Answer>(sql, new { QuestionId = questionId });
        }

        // Создать ответ
        public async Task<int> CreateAnswerAsync(Answer answer)
        {
            var sql = @"
                INSERT INTO Answers (QuestionId, Text, IsCorrect, OrderNumber)
                VALUES (@QuestionId, @Text, @IsCorrect, @OrderNumber);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.ExecuteScalarAsync<int>(sql, answer);
        }

        // Обновить ответ
        public async Task UpdateAnswerAsync(Answer answer)
        {
            var sql = @"
                UPDATE Answers 
                SET Text = @Text, 
                    IsCorrect = @IsCorrect, 
                    OrderNumber = @OrderNumber
                WHERE Id = @Id";

            await _db.ExecuteAsync(sql, answer);
        }

        // Удалить ответ
        public async Task DeleteAnswerAsync(int id)
        {
            var sql = "DELETE FROM Answers WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        // Получить вопрос с ответами
        public async Task<Question> GetQuestionWithAnswersAsync(int questionId)
        {
            var question = await GetQuestionByIdAsync(questionId);
            if (question != null)
            {
                question.Answers = (await GetAnswersByQuestionIdAsync(questionId)).ToList();
            }
            return question;
        }
    }
}