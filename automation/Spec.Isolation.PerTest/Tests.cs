using System;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using NUnit.Framework;
using Spec.Common;
using Todolist;

namespace Spec.Isolation.PerTest;

public static class Tests
{
    [Test]
    public static async Task Migrations()
    {
        await using var database = await SystemUnderTest.StartDatabase();
        var app = App.Setup("-m",
            $"ConnectionStrings__events=\"{database.ConnectionString}\"");

        await app.Invoke();

        await using var connection = database.OpenConnection();
        var result = await connection.QueryAsync<EventStreamEntry>(
            "select stream_id as StreamId, bucket as Bucket, payload as Payload, meta_data as MetaData, position as Position from event_streams");
        result.Should().BeEmpty();
    }

    private sealed record EventStreamEntry(
        Guid StreamId,
        string Bucket,
        string Payload,
        string MetaData,
        long Position);
}