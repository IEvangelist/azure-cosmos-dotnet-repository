// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Validators;
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
        public void NullIOptionsThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => _validator.ValidateForContainerCreation(null));

        [Fact]
        public void NullCosmosConnectionStringThrowsArgumentNullException()
        {
            //Arrange
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => _validator.ValidateForContainerCreation(_options.Object));
        }

        [Fact]
        public void NullDatabaseIdWhenItemPerContainerTypeThrowsArgumentNullException()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = "Some Connection String";
            _repositoryOptions.DatabaseId = null;
            _repositoryOptions.ContainerPerItemType = false;
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => _validator.ValidateForContainerCreation(_options.Object));
        }

        [Fact]
        public void NullContainerIdWhenItemPerContainerTypeThrowsArgumentNullException()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = "Some Connection String";
            _repositoryOptions.DatabaseId = "Database 1";
            _repositoryOptions.ContainerId = null;
            _repositoryOptions.ContainerPerItemType = false;
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => _validator.ValidateForContainerCreation(_options.Object));
        }

        [Fact]
        public void ValidSettingsWithItemPerContainerTimeDoesNotThrow()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = "Some Connection String";
            _repositoryOptions.ContainerPerItemType = true;
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            _validator.ValidateForContainerCreation(_options.Object);
        }

        [Fact]
        public void ValidSettingsWithoutItemPerContainerTimeDoesNotThrow()
        {
            //Arrange
            _repositoryOptions.CosmosConnectionString = "Some Connection String";
            _repositoryOptions.ContainerPerItemType = false;
            _repositoryOptions.DatabaseId = "Database 1";
            _repositoryOptions.ContainerId = "Container";
            _options.Setup(o => o.Value).Returns(_repositoryOptions);

            _validator.ValidateForContainerCreation(_options.Object);
        }
    }
}