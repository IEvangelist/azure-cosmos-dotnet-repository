// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Validators;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Validators
{
    public class DefaultRepositoryOptionsValidatorTests
    {
        private readonly Mock<IOptions<RepositoryOptions>> _options = new();

        private readonly RepositoryOptions _repositoryOptions = new();

        private readonly DefaultRepositoryOptionsValidator _validator = new();

        [Fact]
        public void NullIOptionsThrowsAggregateException() =>
            Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(null));

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

        [Fact]
        public void NullDatabaseIdWhenUsingConnectionStringAuthenticationAndItemPerContainerTypeThrowsAggregateException()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = "Some Connection String";
            _repositoryOptions.TokenCredential = null;
            _repositoryOptions.DatabaseId = null;
            _repositoryOptions.ContainerPerItemType = false;
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            //Act
            //Assert
            Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
        }

        [Fact]
        public void NullContainerIdWhenUsingConnectionStringAuthenticationAndItemPerContainerTypeThrowsAggregateException()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = "Some Connection String";
            _repositoryOptions.TokenCredential = null;
            _repositoryOptions.DatabaseId = "Database 1";
            _repositoryOptions.ContainerId = null;
            _repositoryOptions.ContainerPerItemType = false;
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            //Act
            //Assert
            Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
        }

        [Fact]
        public void NullDatabaseIdWhenUsingTokenCredentialAuthenticationAndItemPerContainerTypeThrowsAggregateException()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = null;
            _repositoryOptions.TokenCredential = new TestTokenCredential();
            _repositoryOptions.AccountEndpoint = "Some Account Endpoint";
            _repositoryOptions.DatabaseId = null;
            _repositoryOptions.ContainerPerItemType = false;
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            //Act
            //Assert
            Assert.Throws<AggregateException>(() => _validator.ValidateForContainerCreation(_options.Object));
        }

        [Fact]
        public void NullContainerIdWhenUsingTokenCredentialAuthenticationAndItemPerContainerTypeThrowsAggregateException()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = null;
            _repositoryOptions.TokenCredential = new TestTokenCredential();
            _repositoryOptions.AccountEndpoint = "Some Account Endpoint";
            _repositoryOptions.DatabaseId = "Database 1";
            _repositoryOptions.ContainerId = null;
            _repositoryOptions.ContainerPerItemType = false;
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
            _repositoryOptions.ContainerPerItemType = true;
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            _validator.ValidateForContainerCreation(_options.Object);
        }

        [Fact]
        public void ValidSettingsUsingConnectionStringAuthenticationWithoutItemPerContainerTimeDoesNotThrow()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = "Some Connection String";
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
}