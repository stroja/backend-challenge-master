namespace Quizzes.Core.Abstractions;
public static class Constants
    {
    public static class Queries
    {
        // TODO: Since these are const values, they are a part of a static class (they could be part of enum as well).
        public const string SelectAllQuizzes = "SELECT * FROM Quiz;";
        public const string SelectAllQuizzesById = "SELECT * FROM Quiz WHERE Id = @Id;";
        public const string SelectAllQuestionsById = "SELECT * FROM Question WHERE QuizId = @QuizId;";
        public const string SelectAnswerByQuizId = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
    }
}

