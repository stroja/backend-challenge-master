namespace Quizzes.Domain.Dtos;

public class AnswerCreateModel
{
    public AnswerCreateModel(string text)
    {
        Text = text;
    }

    public string Text { get; set; }
}