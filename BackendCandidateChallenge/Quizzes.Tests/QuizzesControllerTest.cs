using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Quizzes.API;
using Quizzes.Domain.Dtos;
using Xunit;

namespace Quizzes.Tests;

public class QuizzesControllerTest
{
    const string QuizApiEndPoint = "/api/quizzes/";

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }
    }

    // TODO: Naming of all tests could be improved, something like Gherkin notation: given_when_then.
    // TODO: For this particular case it could be: QuizExists_GetQuiz_Ok.
    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 1;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
            Assert.Equal(quizId, quiz.Id);
            Assert.Equal("My first quiz", quiz.Title);
        }
    }

    // TODO: Naming of all tests could be improved, something like Gherkin notation: given_when_then.
    // TODO: For this particular case it could be: QuizDoesNotExist_GetQuiz_NotFound.
    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));

            // TODO: Should be InternalServerError instead, since the code returns InvalidOperationException.
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    // TODO: Naming of all tests could be improved, something like Gherkin notation: given_when_then.
    // TODO: For this particular case it could be: QuizDoesNotExist_PostQuestion_NotFound.
    [Fact]
        
    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const string QuizApiEndPoint = "/api/quizzes/999/questions";

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var question = new QuestionCreateModel("The answer to everything is what?");
            var content = new StringContent(JsonConvert.SerializeObject(question));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),content);

            // TODO: Should be InternalServerError instead, since the code returns SqliteException.
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]

    public async Task QuizDoesNotExist_CreateCompleteQuizWithTwoQuestionsAndTwoAnswersEach_Created()
    {
        const string QuizApiEndPoint = "/api/quizzes";
        const int quizId = 3;

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();

            var quiz = new QuizCreateModel("My generated Quiz.");
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseQuiz = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"), content);
            Assert.Equal(HttpStatusCode.Created, responseQuiz.StatusCode);
            Assert.NotNull(responseQuiz.Headers.Location);

            for (int i = 1; i < 3; i++)
            {
                var questionPostApiEndPoint = $"/api/quizzes/{quizId}/questions";
                var question = new QuestionCreateModel($"This is question {i}?");
                content = new StringContent(JsonConvert.SerializeObject(question));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var responseQuestion = await client.PostAsync(new Uri(testHost.BaseAddress, questionPostApiEndPoint), content);
                Assert.Equal(HttpStatusCode.Created, responseQuestion.StatusCode);
                Assert.NotNull(responseQuestion.Headers.Location);

                var questionPutApiEndPoint = $"/api/quizzes/{quizId}/questions/{i + 2}";
                var questionUpdate = new QuestionUpdateModel { CorrectAnswerId = 1, Text= $"Is this updated question {i}?" };
                content = new StringContent(JsonConvert.SerializeObject(questionUpdate));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var responseQuestionUpdate = await client.PutAsync(new Uri(testHost.BaseAddress, questionPutApiEndPoint), content);
                Assert.Equal(HttpStatusCode.NoContent, responseQuestionUpdate.StatusCode);

                for (int j = 1; j < 3; j++)
                {
                    var answerPostApiEndPoint = $"/api/quizzes/{quizId}/questions/{i + 2}/answers";
                    var answer = new AnswerCreateModel($"This is answer {j}.");
                    content = new StringContent(JsonConvert.SerializeObject(answer));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var responseAnswer = await client.PostAsync(new Uri(testHost.BaseAddress, answerPostApiEndPoint), content);
                    Assert.Equal(HttpStatusCode.Created, responseQuestion.StatusCode);
                    Assert.NotNull(responseAnswer.Headers.Location);
                }
            }
        }
    }
}