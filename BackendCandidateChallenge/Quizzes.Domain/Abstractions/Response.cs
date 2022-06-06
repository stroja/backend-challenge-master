using System.Net;

namespace Quizzes.Domain.Abstractions
{
    public struct Response<T>
    {
        public Response(HttpStatusCode statusCode, T value, string errorMessage = null)
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            Value = value;
        }

        public HttpStatusCode StatusCode { get; }
        public T Value { get; }
        public string ErrorMessage { get; }
    }
}
