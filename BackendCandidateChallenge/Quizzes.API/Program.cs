using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Quizzes.API;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    // TODO: Add swagger.
    // TODO: Consider using minimalist api since this is .net6.
    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
}