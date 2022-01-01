using FluentMigrator;
using JetBrains.Annotations;

namespace Todolist.Migrations;

[Migration(1)]
[UsedImplicitly]
public sealed class EventStreams : Migration
{
    private static class Columns
    {
        public const string StreamId = "stream_id";
        public const string Position = "position";
        public const string Bucket = "bucket";
    }

    private const string TableName = "event_streams";

    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn(Columns.StreamId).AsGuid().NotNullable()
            .WithColumn(Columns.Bucket).AsString().NotNullable()
            .WithColumn("payload").AsCustom("JSON").NotNullable()
            .WithColumn("meta_data").AsCustom("JSON").NotNullable()
            .WithColumn(Columns.Position).AsInt64().NotNullable();

        Create.UniqueConstraint()
            .OnTable(TableName)
            .Columns(Columns.StreamId, Columns.Bucket, Columns.Position);
    }

    public override void Down() =>
        Delete.Table(TableName);
}