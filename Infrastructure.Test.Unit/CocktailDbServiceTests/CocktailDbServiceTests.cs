using AutoFixture.Xunit2;
using FluentAssertions;
using Infrastructure.CocktailDbService;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Test.Shared;
using Xunit;

namespace Infrastructure.Test.Unit.CocktailDbServiceTests
{
    public class CocktailDbServiceTests
    {
        public class ConstructorTests
        {
            [Fact]
            public void GivenNullHttpClient_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new CocktailDbService.CocktailDbService(null));
            }
        }
        
        public class GetCocktailTests
        {
            private readonly MockHttpClient _mockHttpClient = new();
            private readonly CocktailDbService.CocktailDbService _cocktailDbService;

            private const string CocktailDbSearchPath = "/api/json/v1/1/search.php";

            public GetCocktailTests()
            {
                _cocktailDbService = new CocktailDbService.CocktailDbService(_mockHttpClient.CreateClient());
            }

            [Theory, AutoData]
            public async Task GivenCocktailName_ShouldCallCocktailDb(string cocktailName, CocktailResponse cocktailResponse)
            {
                var stringContent = JsonConvert.SerializeObject(cocktailResponse);
                _mockHttpClient.Returns(CocktailDbSearchPath, HttpStatusCode.OK, stringContent);

                var result = await _cocktailDbService.GetCocktail(cocktailName);

                result.Should().BeEquivalentTo(cocktailResponse);
            }

            [Theory, AutoData]
            public async Task GivenErrorResponseCode_ShouldThrowException(string cocktailName, CocktailResponse cocktailResponse)
            {
                var stringContent = JsonConvert.SerializeObject(cocktailResponse);
                _mockHttpClient.Returns(CocktailDbSearchPath, HttpStatusCode.InternalServerError, stringContent);

                Func<Task> subject = async () => await _cocktailDbService.GetCocktail(cocktailName);

                await subject.Should().ThrowAsync<Exception>();
            }
        }
    }
}