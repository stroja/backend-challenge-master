namespace Quizzes.Domain.Dtos;

public class QuestionCreateModel
{
    public QuestionCreateModel(string text)
    {
        Text = text;
    }

    public string Text { get; set; }
}