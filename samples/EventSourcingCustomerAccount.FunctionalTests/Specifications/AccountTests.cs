// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http.Json;
using Bogus;
using Chill;
using EventSourcingCustomerAccount.FunctionalTests.Fixtures;
using EventSourcingCustomerAccount.Items;
using EventSourcingCustomerAccount.Requests;
using EventSourcingCustomerAccount.Services;
using FluentAssertions;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace EventSourcingCustomerAccount.FunctionalTests.Specifications;

public static class AccountTests
{
    private static Faker<CreateCustomerAccountRequest> CreateCustomerAccountRequestFaker =>
        new Faker<CreateCustomerAccountRequest>()
            .CustomInstantiator(
                f =>
                {
                    var firstName = f.Name.FirstName();
                    var surname = f.Name.LastName();

                    return new CreateCustomerAccountRequest(
                        f.Internet.UserName(
                            firstName,
                            surname),
                        f.Internet.Email(
                            firstName,
                            surname),
                        firstName,
                        surname);
                });

    private static Faker<AssignCustomersAccountAddressRequest> AssignCustomersAccountAddressRequestFaker =>
        new Faker<AssignCustomersAccountAddressRequest>()
            .CustomInstantiator(
                f =>
                {
                    var firstName = f.Name.FirstName();
                    var surname = f.Name.LastName();

                    return new AssignCustomersAccountAddressRequest(
                        f.Internet.UserName(
                            firstName,
                            surname),
                        f.Address.StreetAddress(),
                        f.Address.City(),
                        f.Address.State(),
                        f.Address.Country(),
                        f.Address.ZipCode());
                });

    public class Given_a_valid_request_When_a_new_customer_account_is_created : GivenWhenThen, IClassFixture<ApiFixture>
    {
        private readonly ApiFixture _api;
        private HttpResponseMessage _httpResponse = null!;
        private CreateCustomerAccountRequest _request = null!;

        public Given_a_valid_request_When_a_new_customer_account_is_created(
            ITestOutputHelper testOutputHelper,
            ApiFixture api)
        {
            _api = api;
            _api.OutputHelper = testOutputHelper;

            Given(() => _request = CreateCustomerAccountRequestFaker.Generate());
            When(
                async () => _httpResponse = await _api.HttpClient.PostAsJsonAsync(
                    "api/accounts/",
                    _request));
        }

        [Fact]
        public void Then_the_response_should_be_successful() =>
            _httpResponse.Should().BeSuccessful();

        [Fact]
        public async Task Then_the_read_projection_should_have_been_created()
        {
            IRepository<CustomerAccountReadItem> repository =
                _api.Services.GetRequiredService<IRepository<CustomerAccountReadItem>>();
            CustomerAccountReadItem projection = await repository.GetAsync(_request.Username);

            projection.Should().BeEquivalentTo(
                new
                {
                    _request.Username,
                    _request.Email,
                    _request.FirstName,
                    _request.Surname
                });
        }
    }

    public class Given_a_customer_has_created_an_account_When_they_give_there_address : GivenWhenThen,
        IClassFixture<ApiFixture>
    {
        private readonly ApiFixture _api;
        private HttpResponseMessage _httpResponse = null!;
        private CreateCustomerAccountRequest _createRequest = null!;
        private AssignCustomersAccountAddressRequest _assignAddressRequest = null!;

        public Given_a_customer_has_created_an_account_When_they_give_there_address(
            ITestOutputHelper testOutputHelper,
            ApiFixture api)
        {
            _api = api;
            _api.OutputHelper = testOutputHelper;

            Given(
                async () =>
                {
                    _createRequest = CreateCustomerAccountRequestFaker.Generate();

                    HttpResponseMessage response = await _api.HttpClient.PostAsJsonAsync(
                        "api/accounts/",
                        _createRequest);
                    response.EnsureSuccessStatusCode();

                    _assignAddressRequest = AssignCustomersAccountAddressRequestFaker.Generate() with
                    {
                        Username = _createRequest.Username
                    };
                });

            When(
                async () => _httpResponse = await _api.HttpClient.PutAsJsonAsync(
                    "api/accounts/address",
                    _assignAddressRequest));
        }

        [Fact]
        public void Then_the_response_should_be_successful() =>
            _httpResponse.Should().BeSuccessful();

        [Fact]
        public void Then_a_welcome_letter_should_be_sent()
        {
            IPostalService postalService =  _api.Services.GetRequiredService<IPostalService>();
            postalService.LastSentWelcomeLetter.Should().BeEquivalentTo(
                new
                {
                    _createRequest.FirstName,
                    _createRequest.Surname,
                    Address = new
                    {
                        _assignAddressRequest.AddressLine1,
                        _assignAddressRequest.AddressLine2,
                        _assignAddressRequest.City,
                        _assignAddressRequest.Country,
                        _assignAddressRequest.PostCode
                    }
                });
        }

        [Fact]
        public async Task Then_the_read_projection_should_have_been_created()
        {
            IRepository<CustomerAccountReadItem> repository =
                _api.Services.GetRequiredService<IRepository<CustomerAccountReadItem>>();
            CustomerAccountReadItem projection = await repository.GetAsync(_createRequest.Username);

            projection.Should().BeEquivalentTo(
                new
                {
                    _createRequest.Username,
                    _createRequest.Email,
                    _createRequest.FirstName,
                    _createRequest.Surname
                });
        }
    }
}