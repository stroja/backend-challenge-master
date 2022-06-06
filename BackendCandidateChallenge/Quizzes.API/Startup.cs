using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quizzes.Core;

namespace Quizzes.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // TODO: Add swagger for easy API access, testing and view.
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddDbContext(Configuration, Env);
        services.AddMvc();
        services.AddSingleton(InitializeDb());
        services.AddControllers();

        services.AddScoped<IQuizService, QuizService>();

        //services.AddDbContext<QuizzesDbContext>(opt => opt.UseInMemoryDatabase("QuizzesDB"));

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private IDbConnection InitializeDb()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        // TODO: I would use Migrations for maintainability and potential future refactor.
        // DbContext would be initialized.
        // Migrations would be stored in Quizzes.Data project.
        // Seeding of data would be done through a Quizzes.Data/DbInitializer.cs class.
        var assembly = typeof(Startup).GetTypeInfo().Assembly;
        var migrationResourceNames = assembly.GetManifestResourceNames()
            .Where(x => x.EndsWith(".sql"))
            .OrderBy(x => x);
        if (!migrationResourceNames.Any()) throw new System.Exception("No migration files found!");
        foreach (var resourceName in migrationResourceNames)
        {
            var sql = GetResourceText(assembly, resourceName);
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        return connection;
    }

    private static string GetResourceText(Assembly assembly, string resourceName)
    {
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}