using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;
using Quizzes.Domain;
using Quizzes.Domain.Model;
using Quizzes.Tests.Entities;
using Quizzes.Domain.Dtos;

namespace Quizzes.Tests;

// TODO: Better name and organizes tests. QuizClient vs QuizzesController can be confusing.
// TODO: Add Categories for tests (unit, integration).
public class QuizClientTests : IClassFixture<QuizServiceApiPact>
{
    private readonly IMockProviderService _mockProviderService;
    private readonly Uri _mockProviderServiceBaseUri;
    private static readonly HttpClient Client = new HttpClient();

    public QuizClientTests(QuizServiceApiPact data)
    {
        _mockProviderService = data.MockProviderService;
        _mockProviderService.ClearInteractions();
        _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
    }

    [Fact]
    public async Task GetQuizzes_WhenSomeQuizzesExists_ReturnsTheQuizzes()
    {
        _mockProviderService.ClearInteractions();
        _mockProviderService
            .Given("There are some quizzes")
            .UponReceiving("A GET request to retrieve the quizzes")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/api/quizzes",
                Headers = new Dictionary<string, object>
                {
                    { "Accept", "application/json" }
                }
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json; charset=utf-8" }
                },
                Body = new[]
                {
                    new
                    {
                        id = 123,
                        title = "This is quiz 123"
                    },
                    new {
                        id = 124,
                        title = "This is quiz 124"
                    }
                }
            });

        var consumer = new QuizClient(_mockProviderServiceBaseUri, Client);

        var result = await consumer.GetQuizzesAsync(CancellationToken.None);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage), result.ErrorMessage);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotEmpty(result.Value);
        Assert.Equal(2, result.Value.Count());

        _mockProviderService.VerifyInteractions();
    }

    [Fact]
    public async Task GetQuiz_WhenAQuizWExists_ReturnsTheQuiz()
    {
        _mockProviderService.ClearInteractions();
        _mockProviderService
            .Given("There are some quizzes")
            .UponReceiving("A GET request to retrieve the quiz")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/api/quizzes/123",
                Headers = new Dictionary<string, object>
                {
                    { "Accept", "application/json" }
                }
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json; charset=utf-8" }
                },
                Body = new
                {
                    id = 123,
                    title = "This is quiz 123"
                }
            });

        var consumer = new QuizClient(_mockProviderServiceBaseUri, Client);

        var result = await consumer.GetQuizAsync(123, CancellationToken.None);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage), result.ErrorMessage);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotEqual(Quiz.NotFound, result.Value);
        Assert.Equal("This is quiz 123", result.Value.Title);

        _mockProviderService.VerifyInteractions();
    }

    [Fact]
    public async Task PostQuiz_Returns201CreatedAndLocationHeader()
    {
        _mockProviderService.ClearInteractions();
        _mockProviderService
            //.Given("There are some quizzes")
            .UponReceiving("A POST quiz request")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/api/quizzes",
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" }
                }
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" },
                    { "Location", PactNet.Matchers.Match.Regex("/api/quizzes/1", "quizzes\\/[0-9]*") }
                }
            });

        var consumer = new QuizClient(_mockProviderServiceBaseUri, Client);

        var result = await consumer.PostQuizAsync(new Quiz { Title = "This is quiz 999" }, CancellationToken.None);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage), result.ErrorMessage);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.NotNull(result.Value);

        _mockProviderService.VerifyInteractions();
    }

    [Fact]
    public async Task PostQuestion_Returns201CreatedAndLocationHeader()
    {
        _mockProviderService.ClearInteractions();
        _mockProviderService
            .Given("There are some quizzes")
            .UponReceiving("A POST request to quiz 123 questions collection")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/api/quizzes/123/questions",
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" }
                }
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" },
                    { "Location", PactNet.Matchers.Match.Regex("/api/quizzes/123/questions/1", "quizzes\\/123\\/questions\\/[0-9]*") }
                }
            });

        var consumer = new QuizClient(_mockProviderServiceBaseUri, Client);

        var result = await consumer.PostQuestionAsync(123, new QuizQuestion { Text = "This is a question" }, CancellationToken.None);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage), result.ErrorMessage);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.NotNull(result.Value);

        _mockProviderService.VerifyInteractions();
    }

    [Fact]
    public async Task PutQuestion_WhenAQuestionWExists_UpdatesTheQuestion()
    {
        _mockProviderService.ClearInteractions();
        _mockProviderService
            .Given("There are some quizzes")
            .UponReceiving("A PUT request to update a quiz question with id = 1")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Put,
                Path = "/api/quizzes/123/questions/1",
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" }
                }
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 204,
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json; charset=utf-8" }
                }
            });

        var consumer = new QuizClient(_mockProviderServiceBaseUri, Client);

        var result = await consumer.PutQuestionAsync(123, 1, new QuizQuestion { Text = "Updated text" }, CancellationToken.None);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage), result.ErrorMessage);
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        Assert.NotEqual(Quiz.NotFound, result.Value);

        _mockProviderService.VerifyInteractions();
    }

    [Fact]
    public async Task PostAnswers_Returns201CreatedAndLocationHeader()
    {
        _mockProviderService.ClearInteractions();
        _mockProviderService
            .Given("There are some quizzes")
            .UponReceiving("A POST request")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/api/quizzes",
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" }
                }
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" },
                    { "Location", PactNet.Matchers.Match.Regex("/api/quizzes/1", "quizzes\\/[0-9]*") }
                },
                Body = new
                {
                    title = "This is quiz 999"
                }
            });

        var consumer = new QuizClient(_mockProviderServiceBaseUri, Client);

        var result = await consumer.PostQuizAsync(new Quiz { Title = "This is quiz 999" }, CancellationToken.None);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage), result.ErrorMessage);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.NotNull(result.Value);

        _mockProviderService.VerifyInteractions();
    }

    [Fact]
    public async Task GivenThatAQuizExistsPostingAnAnswerCreatesAQuizResponse()
    {
        _mockProviderService.ClearInteractions();
        _mockProviderService
            .Given("There is a quiz with id '123'")
            .UponReceiving("A POST request creates a quiz response")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/api/quizzes/123/responses",
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" }
                }
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, object>
                {
                    { "Content-Type", "application/json" },
                    { "Location", PactNet.Matchers.Match.Regex("/api/quizzes/123/responses/1", "responses\\/[0-9]*") }
                }
            });

        var consumer = new QuizClient(_mockProviderServiceBaseUri, Client);

        var result = await consumer.PostQuizResponseAsync(new QuestionResponse(), 123);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage), result.ErrorMessage);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.NotNull(result.Value);

        _mockProviderService.VerifyInteractions();
    }

    //[Fact]
    //public async Task PostQuiz_TakeQuiz_VerifyCorrectAnswersScore()
    ////{
    //    _mockProviderService.ClearInteractions();
    //    int quizId = 99999;

    //    // Create quizz.
    //    _mockProviderService
    //        .Given($"There is no quiz with id {quizId}")
    //        .UponReceiving($"A POST quiz request for {quizId}")
    //        .With(new ProviderServiceRequest
    //        {
    //            Method = HttpVerb.Post,
    //            Path = "/api/quizzes",
    //            Headers = new Dictionary<string, object>
    //            {
    //                { "Content-Type", "application/json" }
    //            }
    //        })
    //        .WillRespondWith(new ProviderServiceResponse
    //        {
    //            Status = 201,
    //            Headers = new Dictionary<string, object>
    //            {
    //                { "Content-Type", "application/json" },
    //                { "Location", PactNet.Matchers.Match.Regex($"/api/quizzes/{quizId}", "quizzes\\/[0-9]*") }
    //            }
    //        });

    //    var consumer = new QuizClient(_mockProviderServiceBaseUri, Client);

    //    var result = await consumer.PostQuizAsync(new Quiz { Id = quizId, Title = $"This is quiz {quizId}" }, CancellationToken.None);
    //    Assert.True(string.IsNullOrEmpty(result.ErrorMessage), result.ErrorMessage);
    //    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    //    Assert.NotNull(result.Value);

    //    // Create 2 questions.
    //    for (int i = 1; i < 3; i++)
    //    {
    //        _mockProviderService
    //        .Given($"There is no question with id {i}")
    //        .UponReceiving($"A POST question request with id {i}")
    //        .With(new ProviderServiceRequest
    //        {
    //            Method = HttpVerb.Post,
    //            Path = $"/api/quizzes/{quizId}/questions",
    //            Headers = new Dictionary<string, object>
    //            {
    //                { "Content-Type", "application/json" }
    //            }
    //        })
    //        .WillRespondWith(new ProviderServiceResponse
    //        {
    //            Status = 201,
    //            Headers = new Dictionary<string, object>
    //            {
    //                { "Content-Type", "application/json" },
    //                { "Location", PactNet.Matchers.Match.Regex($"/api/quizzes/{quizId}/questions/{i}", $"quizzes\\/{quizId}\\/questions\\/[0-9]*") }
    //            }
    //        });

    //        var resultQuestions = await consumer.PostQuestionAsync(quizId, new QuizQuestion { Text = $"This is a question no.{i}" }, CancellationToken.None);
    //        Assert.True(string.IsNullOrEmpty(resultQuestions.ErrorMessage), resultQuestions.ErrorMessage);
    //        Assert.Equal(HttpStatusCode.Created, resultQuestions.StatusCode);
    //        Assert.NotNull(resultQuestions.Value);


    //        // Update correctAnswerId for each question.
    //        _mockProviderService
    //       .Given($"There is no set correctAnswerId for question with id {i}")
    //       .UponReceiving($"A PUT request to quiz {quizId} questions collection for question id {i}")
    //       .With(new ProviderServiceRequest
    //       {
    //           Method = HttpVerb.Put,
    //           Path = $"/api/quizzes/{quizId}/questions/{i}",
    //           Headers = new Dictionary<string, object>
    //           {
    //                { "Content-Type", "application/json" }
    //           }
    //       })
    //       .WillRespondWith(new ProviderServiceResponse
    //       {
    //           Status = 201,
    //           Headers = new Dictionary<string, object>
    //           {
    //                { "Content-Type", "application/json" },
    //                { "Location", PactNet.Matchers.Match.Regex($"/api/quizzes/{quizId}/questions/{i}", $"quizzes\\/{quizId}\\/questions\\/[0-9]*") }
    //           }
    //       });

    //        var resultQuestionUpdated = await consumer.PutQuestionAsync(quizId, i, new QuestionUpdateModel { CorrectAnswerId = i == 1 ? 1 : 3 }, CancellationToken.None);
    //        Assert.True(string.IsNullOrEmpty(resultQuestionUpdated.ErrorMessage), resultQuestionUpdated.ErrorMessage);
    //        Assert.Equal(HttpStatusCode.Created, resultQuestionUpdated.StatusCode);


    //        // Create 2 answers for each question.
    //        _mockProviderService
    //        .Given($"There is a question with id {i} and we have the first answer")
    //        .UponReceiving($"A POST request for the question")
    //        .With(new ProviderServiceRequest
    //        {
    //            Method = HttpVerb.Post,
    //            Path = $"/api/quizzes/{quizId}/questions/{i}/answers",
    //            Headers = new Dictionary<string, object>
    //            {
    //                { "Content-Type", "application/json" }
    //            }
    //        })
    //        .WillRespondWith(new ProviderServiceResponse
    //        {
    //            Status = 201,
    //            Headers = new Dictionary<string, object>
    //            {
    //                { "Content-Type", "application/json" },
    //                { "Location", PactNet.Matchers.Match.Regex($"/api/quizzes/{quizId}/questions/{i}/answers", $"quizzes\\/{quizId}\\/questions\\/{i}\\/answers") }
    //            }
    //        });

    //        var resultAnswerFirst = await consumer.PostAnswerAsync(quizId, i, new Answer { Text = $"This is the first answer for question {i}", QuestionId = i }, CancellationToken.None);
    //        Assert.True(string.IsNullOrEmpty(resultAnswerFirst.ErrorMessage), resultAnswerFirst.ErrorMessage);
    //        Assert.Equal(HttpStatusCode.Created, resultAnswerFirst.StatusCode);
    //        Assert.NotNull(resultAnswerFirst.Value);
    //    }


    //    _mockProviderService.VerifyInteractions();
    //}
}