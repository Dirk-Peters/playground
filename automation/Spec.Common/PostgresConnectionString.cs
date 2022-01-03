namespace Spec.Common;

public sealed record PostgresConnectionString(int Port, string Password, string DatabaseName)
{
    public override string ToString() =>
        $"User id=postgres;Password={Password};Host=localhost;Port={Port};Database={DatabaseName}";
}