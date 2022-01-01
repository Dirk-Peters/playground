using System.Reflection;
using CommandLine;
using FluentMigrator.Runner;

namespace Todolist;

public static class App
{
    public static Func<Task> Setup(params string[] args) =>
        Parser.Default.ParseArguments<Options>(args)
            .MapResult<Options, Func<Task>>(
                o => o.RunMigrations
                    ? () => Migrate(Builder(args))
                    : () => Serve(Builder(args)),
                _ => () => Task.CompletedTask);

    private static WebApplicationBuilder Builder(string[] args) =>
        WebApplication.CreateBuilder(args);

    private static async Task Serve(WebApplicationBuilder builder)
    {
        await using var app = builder.Build();
        app.MapGet("/", () => "Hello World!");
        await app.RunAsync();
    }

    private static async Task Migrate(WebApplicationBuilder builder)
    {
        builder.Services
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c
                .AddPostgres()
                .WithGlobalConnectionString(builder.Configuration.GetConnectionString("events"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.All());
        await using var app = builder.Build();
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();
        runner.MigrateUp();
    }
}