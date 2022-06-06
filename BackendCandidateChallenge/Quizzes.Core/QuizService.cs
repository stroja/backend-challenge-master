using System.Data;
using Dapper;
using Quizzes.Core.Abstractions;
using Quizzes.Domain.Dtos;
using Quizzes.Domain.Entities;

namespace Quizzes.Core;
// TODO: If there are many services in the future, this could be in a separate folder called Quizzes which would have subfolder Dtos.
// It would look like this:
// Quizzes
//      Dtos        -> this would be a folder for different Quiz Dtos.
//      Validators  -> this would be a folder for Quiz validators.
//      QuizService.cs
//      IQuizService.cs
//      MappingProfile.cs -> this would have the mapping between the entity and dto.

// TODO: Implement logging as well.
public class QuizService : IQuizService
{
    private readonly IDbConnection _connection;

    public QuizService(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<QuizResponseModel>> GetAllAsync()
    {
        var quizzes = _connection.Query<Quiz>(Constants.Queries.SelectAllQuizzes);
        return quizzes.Select(quiz =>
            new QuizResponseModel
            {
                Id = quiz.Id,
                Title = quiz.Title
            }).ToList();
    }

    public async Task<QuizResponseModel> GetAsync(int id)
    {
        try
        {
            var quiz = _connection.QuerySingle<Quiz>(Constants.Queries.SelectAllQuizzesById, new { Id = id });
            var questions = _connection.Query<Question>(Constants.Queries.SelectAllQuestionsById, new { QuizId = id });
            var answers = _connection.Query<Answer>(Constants.Queries.SelectAnswerByQuizId, new { QuizId = id })
                .Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) =>
                {
                    if (!dict.ContainsKey(answer.QuestionId))
                        dict.Add(answer.QuestionId, new List<Answer>());
                    dict[answer.QuestionId].Add(answer);
                    return dict;
                });
            return new QuizResponseModel
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Questions = questions.Select(question => new QuizResponseModel.QuestionItem
                {
                    Id = question.Id,
                    Text = question.Text,
                    Answers = answers.ContainsKey(question.Id)
                        ? answers[question.Id].Select(answer => new QuizResponseModel.AnswerItem
                        {
                            Id = answer.Id,
                            Text = answer.Text
                        })
                        : Array.Empty<QuizResponseModel.AnswerItem>(),
                    CorrectAnswerId = question.CorrectAnswerId
                }),
                Links = new Dictionary<string, string>
            {
                {"self", $"/api/quizzes/{id}"},
                {"questions", $"/api/quizzes/{id}/questions"}
            }
            };
        }
        catch (Exception)
        {
            throw;
        }
    }
}
