// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed;

public class ChangeFeedOptionsTests
{
    [Fact]
    public void StarTime_Utc_SetsValue()
    {
        //Arrange
        var startTime = new DateTime(2000, 1, 1, 0, 0,0, DateTimeKind.Utc);

        //Act
        var actual = new ChangeFeedOptions(typeof(object)) { StartTime = startTime };

        //Assert
        Assert.Equal(startTime, actual.StartTime);
    }

    [Fact]
    public void StartTime_NotUtc_Throws()
    {
        //Arrange
        var startTime = new DateTime(2000, 1, 1);

        //Act
        //Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChangeFeedOptions(typeof(object)) { StartTime = startTime });
    }

    [Fact]
    public void StartTime_Null_SetsValue()
    {
        //Arrange
        //Act
        var actual = new ChangeFeedOptions(typeof(object)) {StartTime = null};

        //Assert
        Assert.Null(actual.StartTime);
    }
}
