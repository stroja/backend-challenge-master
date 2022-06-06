using Quizzes.Domain.Dtos;

namespace Quizzes.Core;

public interface IQuizService
{
    Task<List<QuizResponseModel>> GetAllAsync();

    Task<QuizResponseModel> GetAsync(int id);
}
