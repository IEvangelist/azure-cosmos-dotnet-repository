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

    [Fact]
    public void LeaseIntervals_SetValues_RoundTrip()
    {
        //Arrange
        var acquire = TimeSpan.FromSeconds(13);
        var expiration = TimeSpan.FromSeconds(120);
        var renew = TimeSpan.FromSeconds(30);

        //Act
        var actual = new ChangeFeedOptions(typeof(object))
        {
            LeaseAcquireInterval = acquire,
            LeaseExpirationInterval = expiration,
            LeaseRenewInterval = renew
        };

        //Assert
        Assert.Equal(acquire, actual.LeaseAcquireInterval);
        Assert.Equal(expiration, actual.LeaseExpirationInterval);
        Assert.Equal(renew, actual.LeaseRenewInterval);
    }

    [Fact]
    public void LeaseIntervals_NotSet_AreNull()
    {
        //Arrange
        //Act
        var actual = new ChangeFeedOptions(typeof(object));

        //Assert
        Assert.Null(actual.LeaseAcquireInterval);
        Assert.Null(actual.LeaseExpirationInterval);
        Assert.Null(actual.LeaseRenewInterval);
    }

    [Fact]
    public void IsTheSameAs_SameLeaseIntervals_ReturnsTrue()
    {
        //Arrange
        var left = new ChangeFeedOptions(typeof(object))
        {
            LeaseAcquireInterval = TimeSpan.FromSeconds(13),
            LeaseExpirationInterval = TimeSpan.FromSeconds(120),
            LeaseRenewInterval = TimeSpan.FromSeconds(30)
        };
        var right = new ChangeFeedOptions(typeof(object))
        {
            LeaseAcquireInterval = TimeSpan.FromSeconds(13),
            LeaseExpirationInterval = TimeSpan.FromSeconds(120),
            LeaseRenewInterval = TimeSpan.FromSeconds(30)
        };

        //Act
        //Assert
        Assert.True(left.IsTheSameAs(right));
    }

    public static IEnumerable<object[]> DifferingLeaseInterval()
    {
        yield return new object[] { new ChangeFeedOptions(typeof(object)) { LeaseAcquireInterval = TimeSpan.FromSeconds(13) } };
        yield return new object[] { new ChangeFeedOptions(typeof(object)) { LeaseExpirationInterval = TimeSpan.FromSeconds(120) } };
        yield return new object[] { new ChangeFeedOptions(typeof(object)) { LeaseRenewInterval = TimeSpan.FromSeconds(30) } };
    }

    [Theory]
    [MemberData(nameof(DifferingLeaseInterval))]
    public void IsTheSameAs_DifferentLeaseInterval_ReturnsFalse(ChangeFeedOptions withInterval)
    {
        //Arrange
        var without = new ChangeFeedOptions(typeof(object));

        //Act
        //Assert
        Assert.False(withInterval.IsTheSameAs(without));
    }
}
