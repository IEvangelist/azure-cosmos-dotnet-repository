// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Validators;

public class DefaultRepositoryOptionsValidatorTests
{
    private readonly Mock<IOptions<RepositoryOptions>> _options = new();

    private readonly RepositoryOptions _repositoryOptions = new();

    private readonly DefaultRepositoryOptionsValidator _validator = new();

    [Fact]
    public void NullCosmosConnectionStringThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = null;
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    [Fact]
    public void NullTokenCredentialThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.TokenCredential = null;
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    [Fact]
    public void NullCosmosConnectionStringAndTokenCredentialThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = null;
        _repositoryOptions.TokenCredential = null;
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    // Bad - nothing provided for either connection method
    [Fact]
    public void NullCosmosConnectionStringAndNullTokenCredentialAndNullAccountEndpointProvidedThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = null;
        _repositoryOptions.TokenCredential = null;
        _repositoryOptions.AccountEndpoint = null;
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    // Bad - everything provided for both connection methods
    [Fact]
    public void NotNullConnectionStringAndNotNullTokenCredentialAndNotNullAccountEndpointProvidedThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = "Some Connection String";
        _repositoryOptions.TokenCredential = new TestTokenCredential();
        _repositoryOptions.AccountEndpoint = "Some Account Endpoint";
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    // Bad - some token auth details provided along connection string, the reverse case is covered above with everything for both connection methods
    [Fact]
    public void NotNullConnectionStringAndNullTokenCredentialAndNotNullAccountEndpointProvidedThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = "Some Connection String";
        _repositoryOptions.TokenCredential = null;
        _repositoryOptions.AccountEndpoint = "Some Account Endpoint";
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    // Bad - some token auth details provided along connection string, the reverse case is covered above with everything for both connection methods
    [Fact]
    public void NotNullConnectionStringAndNotNullTokenCredentialAndNullAccountEndpointProvidedThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = "Some Connection String";
        _repositoryOptions.TokenCredential = new TestTokenCredential();
        _repositoryOptions.AccountEndpoint = null;
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    // Bad - endpoint without token
    [Fact]
    public void NotNullAccountEndpointProvidedWithNullTokenCredentialThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = null;
        _repositoryOptions.TokenCredential = null;
        _repositoryOptions.AccountEndpoint = "Some Account Endpoint";
        _repositoryOptions.ContainerPerItemType = false;
        _repositoryOptions.DatabaseId = "Database 1";
        _repositoryOptions.ContainerId = "Container";
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    // Bad - token without endpoint
    [Fact]
    public void NotNullTokenCredentialProvidedWithNullAccountEndpointThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = null;
        _repositoryOptions.TokenCredential = new TestTokenCredential();
        _repositoryOptions.AccountEndpoint = null;
        _repositoryOptions.ContainerPerItemType = false;
        _repositoryOptions.DatabaseId = "Database 1";
        _repositoryOptions.ContainerId = "Container";
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    [Fact]
    public void NullAccountEndpointWhenUsingTokenCredentialAuthenticationThrowsAggregateException()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = null;
        _repositoryOptions.TokenCredential = new TestTokenCredential();
        _repositoryOptions.AccountEndpoint = null;
        _repositoryOptions.DatabaseId = "Database 1";
        _repositoryOptions.ContainerPerItemType = true;
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        //Act
        //Assert
        Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
    }

    [Fact]
    public void ValidSettingsUsingConnectionStringAuthenticationWithItemPerContainerTimeDoesNotThrow()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = "Some Connection String";
        _repositoryOptions.AccountEndpoint = null;
        _repositoryOptions.TokenCredential = null;
        _repositoryOptions.ContainerPerItemType = true;
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        _validator.ValidateForContainerCreation(_options.Object);
    }

    [Fact]
    public void ValidSettingsUsingConnectionStringAuthenticationWithoutItemPerContainerTimeDoesNotThrow()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = "Some Connection String";
        _repositoryOptions.AccountEndpoint = null;
        _repositoryOptions.TokenCredential = null;
        _repositoryOptions.ContainerPerItemType = false;
        _repositoryOptions.DatabaseId = "Database 1";
        _repositoryOptions.ContainerId = "Container";
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        _validator.ValidateForContainerCreation(_options.Object);
    }

    [Fact]
    public void ValidSettingsUsingTokenCredentialAuthenticationWithItemPerContainerTimeDoesNotThrow()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = null;
        _repositoryOptions.TokenCredential = new TestTokenCredential();
        _repositoryOptions.AccountEndpoint = "Some Account Endpoint";
        _repositoryOptions.ContainerPerItemType = true;
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        _validator.ValidateForContainerCreation(_options.Object);
    }

    [Fact]
    public void ValidSettingsUsingTokenCredentialAuthenticationWithoutItemPerContainerTimeDoesNotThrow()
    {
        //Arrange
        _repositoryOptions.CosmosConnectionString = null;
        _repositoryOptions.TokenCredential = new TestTokenCredential();
        _repositoryOptions.AccountEndpoint = "Some Account Endpoint";
        _repositoryOptions.ContainerPerItemType = false;
        _repositoryOptions.DatabaseId = "Database 1";
        _repositoryOptions.ContainerId = "Container";
        _options.Setup(o => o.Value).Returns(_repositoryOptions);

        _validator.ValidateForContainerCreation(_options.Object);
    }
}