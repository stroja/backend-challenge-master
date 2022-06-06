namespace Quizzes.Tests.Entities;

public struct Quiz
{
    public int Id;
    public string Title;
    public static Quiz NotFound => default(Quiz);
}
