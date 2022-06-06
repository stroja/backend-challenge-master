using Microsoft.EntityFrameworkCore;
using Quizzes.Domain.Entities;
namespace Quizzes.Data;

// TODO: I would have migrations created in code, easily maintainable and ran with one line of code.
public class QuizzesDbContext : DbContext
{
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }

    public QuizzesDbContext(DbContextOptions<QuizzesDbContext> options) : base(options)
    {
    }

    // TODO: This is called when the DBContext is initialized.
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Answer>().ToTable("Answer");
    //    modelBuilder.Entity<Question>().ToTable("Question");
    //    modelBuilder.Entity<Quiz>().ToTable("Quiz");
    //}
}
