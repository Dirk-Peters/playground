using CommandLine;
using JetBrains.Annotations;

namespace Todolist;

public sealed class Options
{
    [Option('m', "migrate", Required = false, HelpText = "run migrations without starting to serve.")]
    public bool RunMigrations { get; [UsedImplicitly] set; }
}