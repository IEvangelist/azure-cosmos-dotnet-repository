// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Items;

public class FullItemTests
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    [Fact]
    public void FullItem_WithNullTimeToLive_SerializesCorrectly()
    {
        //Arrange
        var etagValue = Guid.NewGuid().ToString();

        TestItem item = new(etagValue);

        //Act
        var json = JsonConvert.SerializeObject(item, _jsonSerializerSettings);

        //Assert
        json.Should().Contain($"\"_etag\":\"{etagValue}\"");
        json.Should().NotContain("timeToLive");
        json.Should().NotContain("ttl");
    }

    [Fact]
    public void FullItem_WithPopulatedTimeToLive_SerializesCorrectly()
    {
        //Arrange
        var etagValue = Guid.NewGuid().ToString();

        TestItem item = new(etagValue)
        {
            TimeToLive = TimeSpan.FromSeconds(10)
        };

        //Act
        var json = JsonConvert.SerializeObject(item, _jsonSerializerSettings);

        //Assert
        json.Should().Contain($"\"_etag\":\"{etagValue}\"");
        json.Should().Contain("\"timeToLive\":\"00:00:10\"");
        json.Should().Contain("\"ttl\":10");
    }
}