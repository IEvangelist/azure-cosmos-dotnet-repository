// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultCosmosClientProviderTests
{
    [Fact]
    public void DefaultCosmosClientProviderCorrectlyDisposesOverloadWithConnectionString()
    {
        var mock = new Mock<ICosmosClientOptionsProvider>();
        mock.SetupGet(provider => provider.ClientOptions).Returns(new CosmosClientOptions());
        var provider =
            new DefaultCosmosClientProvider(
                mock.Object,
                Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                {
                    CosmosConnectionString =
                        "AccountEndpoint=https://localtestcosmos.documents.azure.com:443/;AccountKey=RmFrZUtleQ==;"
                }));

        provider.Dispose();

        Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await provider.UseClientAsync(client => client.ReadAccountAsync()));
    }

    [Fact]
    public void DefaultCosmosClientProviderCorrectlyDisposesOverloadWithTokenCredential()
    {
        var mock = new Mock<ICosmosClientOptionsProvider>();
        mock.SetupGet(provider => provider.ClientOptions).Returns(new CosmosClientOptions());
        var provider =
            new DefaultCosmosClientProvider(
                mock.Object,
                Microsoft.Extensions.Options.Options.Create(new RepositoryOptions
                {
                    TokenCredential = new TestTokenCredential(),
                    AccountEndpoint = "https://localtestcosmos.documents.azure.com:443/"
                }));

        provider.Dispose();

        Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await provider.UseClientAsync(client => client.ReadAccountAsync()));
    }
}
