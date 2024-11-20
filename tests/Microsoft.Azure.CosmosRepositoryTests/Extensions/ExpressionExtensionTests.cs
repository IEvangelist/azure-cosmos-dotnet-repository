// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Extensions;

public class ExpressionExtensionTests
{
    public static IEnumerable<object[]> CompositionInput => new[]
    {
        new object[] { new DateTime(1970, 4, 20), true },
        [default(DateTime), false],
        [DateTime.Now, false]
    };

    [
        Theory,
        MemberData(nameof(CompositionInput))
    ]
    public void ComposeCorrectlyAccountsForBothExpressions(DateTime arg, bool expected)
    {
        Expression<Func<DateTime, bool>> isOlderThanMe = date => date < new DateTime(1984, 7, 7);
        Expression<Func<DateTime, bool>> composition =
            isOlderThanMe.Compose(date => date != DateTime.MinValue, Expression.AndAlso);

        Assert.Equal(expected, composition.Compile().Invoke(arg));
    }
}