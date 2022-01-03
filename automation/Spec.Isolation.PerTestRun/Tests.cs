using System;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using NUnit.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Spec.Isolation.PerTestRun;

public sealed class Tests : IResetDatabaseAfterTest
{
    [Test]
    public async Task Insert_Once()
    {
        var connection = GlobalSystemUnderTest.Instance.OpenConnection();
        await connection.ExecuteAsync(
            "insert into event_streams (stream_id,bucket,position,payload,meta_data) " +
            " Values (@streamId,@bucket,@position,CAST(@payload as JSON),CAST(@metaData as JSON))",
            new
            {
                streamId = Guid.NewGuid(),
                bucket = "list",
                position = 0,
                payload = JsonSerializer.Serialize(new { }),
                metaData = JsonSerializer.Serialize(new
                {
                    EventName = "ListCreated",
                    TimeStamp = DateTime.Now
                })
            });
        var result = await connection.QueryAsync<dynamic>("select * from event_streams");
        result.Should().HaveCount(1);
    }

    [Test, Repeat(100)]
    public async Task Insert_Repeat() =>
        await Insert_Once();
}