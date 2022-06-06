namespace Quizzes.Domain.Dtos;

public class QuizResponseModel
{
    public class AnswerItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class QuestionItem
    {
        public int Id { get; set; }
        public string Text { get; set; }

        // TODO: Should have this as nullable.
        public IEnumerable<AnswerItem> Answers { get; set; }
        public int CorrectAnswerId { get; set; }
    }

    public long Id { get; set; }
    public string Title { get; set; }

    // TODO: Should have this as nullable.
    public IEnumerable<QuestionItem> Questions { get; set; }

    // TODO: Should have this as nullable.
    public IDictionary<string, string> Links { get; set; }
}