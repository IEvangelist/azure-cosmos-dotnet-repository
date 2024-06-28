// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Paging;

class Dog : FullItem
{
    public string Breed { get; }
    public string Name { get; private set; }

    protected override string GetPartitionKeyValue() => Breed;


    public Dog(string breed, string name = "dasher")
    {
        Breed = breed;
        Name = name;
    }
}

public partial class PagingTests
{
    [Fact]
    public void Page_InitializedConstructorWithRequiredValues_FillProperties()
    {
        //Arrange
        var total = 1;
        var size = 25;
        double charge = 1;
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("golden retriever");
        List<Dog> dogs = [dog1, dog2];

        //Act
        Page<Dog> pageOfDogs = new(total, size, dogs.AsReadOnly(), charge);

        //Assert
        Assert.Equal(total, pageOfDogs.Total);
        Assert.Equal(size, pageOfDogs.Size);
        Assert.Equal(charge, pageOfDogs.Charge);
        Assert.Equal(default!, pageOfDogs.Continuation);
        Assert.Equal(pageOfDogs.Items, dogs.AsReadOnly());
    }

    [Fact]
    public void Page_InitializedConstructorWithRequiredAndNonRequiredValues_FillProperties()
    {
        //Arrange
        var total = 1;
        var size = 25;
        double charge = 1;
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("golden retriever");
        List<Dog> dogs = [dog1, dog2];
        var continuationToken = "ContinuationToken";

        //Act
        Page<Dog> pageOfDogs = new(total, size, dogs.AsReadOnly(), charge, continuationToken);

        //Assert
        Assert.Equal(total, pageOfDogs.Total);
        Assert.Equal(size, pageOfDogs.Size);
        Assert.Equal(charge, pageOfDogs.Charge);
        Assert.Equal(continuationToken, pageOfDogs.Continuation);
        Assert.Equal(pageOfDogs.Items, dogs.AsReadOnly());
    }

    [Fact]
    public void PageExtended_InitializedConstructorWithRequiredValues_FillProperties()
    {
        //Arrange
        var total = 1;
        var size = 25;
        double charge = 1;
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("golden retriever");
        List<Dog> dogs = [dog1, dog2];

        //Act
        PageQueryResult<Dog> pageOfDogs = new(total, size, dogs.AsReadOnly(), charge);

        //Assert
        Assert.Equal(total, pageOfDogs.Total);
        Assert.Equal(size, pageOfDogs.Size);
        Assert.Equal(charge, pageOfDogs.Charge);
        Assert.Equal(default!, pageOfDogs.PageNumber);
        Assert.Equal(default!, pageOfDogs.Continuation);
        Assert.Equal(pageOfDogs.Items, dogs.AsReadOnly());
    }

    [Fact]
    public void PageExtended_InitializedConstructorWithRequiredAndNonRequiredValues_FillProperties()
    {
        //Arrange
        var total = 1;
        var size = 25;
        var pageNumber = 1;
        double charge = 1;
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("golden retriever");
        List<Dog> dogs = [dog1, dog2];
        var continuationToken = "ContinuationToken";

        //Act
        PageQueryResult<Dog> pageOfDogs = new(total, pageNumber, size, dogs.AsReadOnly(), charge, continuationToken);

        //Assert
        Assert.Equal(total, pageOfDogs.Total);
        Assert.Equal(pageNumber, pageOfDogs.PageNumber);
        Assert.Equal(size, pageOfDogs.Size);
        Assert.Equal(charge, pageOfDogs.Charge);
        Assert.Equal(continuationToken, pageOfDogs.Continuation);
        Assert.Equal(pageOfDogs.Items, dogs.AsReadOnly());
    }

    [Fact]
    public void PageExtended_InitializedConstructorWithFirstPage_FillProperties()
    {
        //Arrange
        var total = 10;
        var size = 5;
        var pageNumber = 1;
        double charge = 1;
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("golden retriever");
        Dog dog3 = new("cocker spaniel");
        Dog dog4 = new("golden retriever");
        Dog dog5 = new("cocker spaniel");
        Dog dog6 = new("golden retriever");
        Dog dog7 = new("cocker spaniel");
        Dog dog8 = new("golden retriever");
        Dog dog9 = new("cocker spaniel");
        Dog dog10 = new("golden retriever");
        List<Dog> dogs = [dog1, dog2, dog3, dog4, dog5, dog6, dog7, dog8, dog9, dog10];
        var continuationToken = "ContinuationToken";
        var expectedTotalPages = 2;
        var expectedNextPage = 2;

        //Act
        PageQueryResult<Dog> pageOfDogs = new(total, pageNumber, size, dogs.AsReadOnly(), charge, continuationToken);

        //Assert
        Assert.Equal(total, pageOfDogs.Total);
        Assert.Equal(pageNumber, pageOfDogs.PageNumber);
        Assert.Equal(size, pageOfDogs.Size);
        Assert.Equal(charge, pageOfDogs.Charge);
        Assert.Equal(continuationToken, pageOfDogs.Continuation);
        Assert.True(pageOfDogs.HasNextPage);
        Assert.False(pageOfDogs.HasPreviousPage);
        Assert.Equal(expectedTotalPages, pageOfDogs.TotalPages);
        Assert.Equal(expectedNextPage, pageOfDogs.NextPageNumber);
        Assert.Equal(pageOfDogs.Items, dogs.AsReadOnly());
    }

    [Fact]
    public void PageExtended_InitializedConstructorWithLastPage_FillProperties()
    {
        //Arrange
        var total = 10;
        var size = 5;
        var pageNumber = 2;
        double charge = 1;
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("golden retriever");
        Dog dog3 = new("cocker spaniel");
        Dog dog4 = new("golden retriever");
        Dog dog5 = new("cocker spaniel");
        Dog dog6 = new("golden retriever");
        Dog dog7 = new("cocker spaniel");
        Dog dog8 = new("golden retriever");
        Dog dog9 = new("cocker spaniel");
        Dog dog10 = new("golden retriever");
        List<Dog> dogs = [dog1, dog2, dog3, dog4, dog5, dog6, dog7, dog8, dog9, dog10];
        var continuationToken = "ContinuationToken";
        var expectedTotalPages = 2;
        var expectedPreviousPage = 1;

        //Act
        PageQueryResult<Dog> pageOfDogs = new(total, pageNumber, size, dogs.AsReadOnly(), charge, continuationToken);

        //Assert
        Assert.Equal(total, pageOfDogs.Total);
        Assert.Equal(pageNumber, pageOfDogs.PageNumber);
        Assert.Equal(size, pageOfDogs.Size);
        Assert.Equal(charge, pageOfDogs.Charge);
        Assert.Equal(continuationToken, pageOfDogs.Continuation);
        Assert.True(pageOfDogs.HasPreviousPage);
        Assert.False(pageOfDogs.HasNextPage);
        Assert.Equal(expectedTotalPages, pageOfDogs.TotalPages);
        Assert.Equal(expectedPreviousPage, pageOfDogs.PreviousPageNumber);
        Assert.Equal(pageOfDogs.Items, dogs.AsReadOnly());
    }


    [Fact]
    public void PageExtended_InitializedConstructorWithIntermediatePage_FillProperties()
    {
        //Arrange
        var total = 10;
        var size = 2;
        var pageNumber = 2;
        double charge = 1;
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("golden retriever");
        Dog dog3 = new("cocker spaniel");
        Dog dog4 = new("golden retriever");
        Dog dog5 = new("cocker spaniel");
        Dog dog6 = new("golden retriever");
        Dog dog7 = new("cocker spaniel");
        Dog dog8 = new("golden retriever");
        Dog dog9 = new("cocker spaniel");
        Dog dog10 = new("golden retriever");
        List<Dog> dogs = [dog1, dog2, dog3, dog4, dog5, dog6, dog7, dog8, dog9, dog10];
        var continuationToken = "ContinuationToken";
        var expectedTotalPages = 5;
        var expectedPreviousPage = 1;
        var expectedNextPage = 3;

        //Act
        PageQueryResult<Dog> pageOfDogs = new(total, pageNumber, size, dogs.AsReadOnly(), charge, continuationToken);

        //Assert
        Assert.Equal(total, pageOfDogs.Total);
        Assert.Equal(pageNumber, pageOfDogs.PageNumber);
        Assert.Equal(size, pageOfDogs.Size);
        Assert.Equal(charge, pageOfDogs.Charge);
        Assert.Equal(continuationToken, pageOfDogs.Continuation);
        Assert.True(pageOfDogs.HasPreviousPage);
        Assert.True(pageOfDogs.HasNextPage);
        Assert.Equal(expectedTotalPages, pageOfDogs.TotalPages);
        Assert.Equal(expectedPreviousPage, pageOfDogs.PreviousPageNumber);
        Assert.Equal(expectedNextPage, pageOfDogs.NextPageNumber);
        Assert.Equal(pageOfDogs.Items, dogs.AsReadOnly());
    }

    [Fact]
    public void PageExtended_InitializedConstructorWithWithoutPageNumber_NextPageIsLastPageAndPreviousPageIsFirstPage()
    {
        //Arrange
        var total = 10;
        var size = 2;
        double charge = 1;
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("golden retriever");
        Dog dog3 = new("cocker spaniel");
        Dog dog4 = new("golden retriever");
        Dog dog5 = new("cocker spaniel");
        Dog dog6 = new("golden retriever");
        Dog dog7 = new("cocker spaniel");
        Dog dog8 = new("golden retriever");
        Dog dog9 = new("cocker spaniel");
        Dog dog10 = new("golden retriever");
        List<Dog> dogs = [dog1, dog2, dog3, dog4, dog5, dog6, dog7, dog8, dog9, dog10];
        var continuationToken = "ContinuationToken";
        var expectedTotalPages = 5;
        var expectedPreviousPage = 1;
        var expectedNextPage = 5;

        //Act
        PageQueryResult<Dog> pageOfDogs = new(total, size, dogs.AsReadOnly(), charge, continuationToken);

        //Assert
        Assert.Equal(total, pageOfDogs.Total);
        Assert.Equal(default!, pageOfDogs.PageNumber);
        Assert.Equal(size, pageOfDogs.Size);
        Assert.Equal(charge, pageOfDogs.Charge);
        Assert.Equal(continuationToken, pageOfDogs.Continuation);
        Assert.False(pageOfDogs.HasPreviousPage);
        Assert.False(pageOfDogs.HasNextPage);
        Assert.Equal(expectedTotalPages, pageOfDogs.TotalPages);
        Assert.Equal(expectedPreviousPage, pageOfDogs.PreviousPageNumber);
        Assert.Equal(expectedNextPage, pageOfDogs.NextPageNumber);
        Assert.Equal(pageOfDogs.Items, dogs.AsReadOnly());
    }
}
