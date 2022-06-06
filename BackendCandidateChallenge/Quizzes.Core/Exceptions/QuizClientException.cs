using System.Net;

namespace Quizzes.Core.Exceptions;

public class QuizClientException : HttpRequestException
{
    public HttpStatusCode ResponseStatusCode { get; }

    public QuizClientException(HttpStatusCode responseStatusCode)
    {
        ResponseStatusCode = responseStatusCode;
    }
}

