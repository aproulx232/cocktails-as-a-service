using AutoFixture.Xunit2;
using FluentAssertions;
using Infrastructure.CocktailDbService;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Application.Test.Unit
{
    public class CocktailProviderTests
    {
        public class ConstructorTests
        {
            [Fact]
            public void GivenNullCocktailDbService_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new CocktailProvider(null));
            }
        }

        public class GetCocktailTests
        {
            private readonly Mock<ICocktailDbService?> _cocktailDbService = new();
            private readonly CocktailProvider _cocktailProvider;

            public GetCocktailTests()
            {
                _cocktailProvider = new CocktailProvider(_cocktailDbService.Object);
            }

            [Theory, AutoData]
            public async Task GivenCocktailName_ShouldCallCocktailDbServiceGetCocktail(string cocktailName, CocktailResponse cocktailResponse)
            {
                _cocktailDbService.Setup(cds => cds!.GetCocktail(cocktailName)).ReturnsAsync(cocktailResponse);

                await _cocktailProvider.GetCocktail(cocktailName);

                _cocktailDbService.VerifyAll();
            }

            [Theory, AutoData]
            public async Task GivenCocktailResponse_ShouldReturnMappedCocktail(string cocktailName, CocktailResponse cocktailResponse)
            {
                _cocktailDbService.Setup(cds => cds!.GetCocktail(cocktailName)).ReturnsAsync(cocktailResponse);

                var result = await _cocktailProvider.GetCocktail(cocktailName);

                result.Name.Should().Be(cocktailResponse.Drinks!.First().Name);
                result.Instructions.Should().Be(cocktailResponse.Drinks!.First().Instructions);
                result.Glass.Should().Be(cocktailResponse.Drinks!.First().Glass);
                result.Thumbnail.Should().Be(cocktailResponse.Drinks!.First().Thumbnail);
                result.Ingredients!.First().Name.Should().Be(cocktailResponse.Drinks!.First().Ingredient1);
                result.Ingredients!.First().Measurement.Should().Be(cocktailResponse.Drinks!.First().Measure1);
                result.Ingredients!.Count.Should().Be(15);
            }

            [Theory, AutoData]
            public async Task GivenNoDrinkResponse_ShouldThrowException(string cocktailName, CocktailResponse cocktailResponse)
            {
                cocktailResponse.Drinks = null;
                _cocktailDbService.Setup(cds => cds!.GetCocktail(cocktailName)).ReturnsAsync(cocktailResponse);

                Func<Task> subject = async () => await _cocktailProvider.GetCocktail(cocktailName);

                await subject.Should().ThrowAsync<Exception>();
            }
            
            [Theory, AutoData]
            public async Task GivenCocktailResponseWithNullIngredients_ShouldNotAddToCocktailObject(string cocktailName, CocktailResponse cocktailResponse)
            {
                cocktailResponse.Drinks!.First().Ingredient1 = null;
                cocktailResponse.Drinks!.First().Measure1 = null;
                _cocktailDbService.Setup(cds => cds!.GetCocktail(cocktailName)).ReturnsAsync(cocktailResponse);

                var result = await _cocktailProvider.GetCocktail(cocktailName);

                result.Ingredients!.Count.Should().Be(14);
            }
        }

        public class GetRandomCocktailTests
        {
            private readonly Mock<ICocktailDbService?> _cocktailDbService = new();
            private readonly CocktailProvider _cocktailProvider;

            public GetRandomCocktailTests()
            {
                _cocktailProvider = new CocktailProvider(_cocktailDbService.Object);
            }

            [Theory, AutoData]
            public async Task GivenCallToGetRandomCocktail_ShouldCallCocktailDbServiceGetCocktail(CocktailResponse cocktailResponse)
            {
                _cocktailDbService.Setup(cds => cds!.GetRandomCocktail()).ReturnsAsync(cocktailResponse);

                await _cocktailProvider.GetRandomCocktail();

                _cocktailDbService.VerifyAll();
            }

            [Theory, AutoData]
            public async Task GivenCallToGetRandomCocktail_ShouldReturnMappedCocktail(CocktailResponse cocktailResponse)
            {
                _cocktailDbService.Setup(cds => cds!.GetRandomCocktail()).ReturnsAsync(cocktailResponse);

                var result = await _cocktailProvider.GetRandomCocktail();

                result.Name.Should().Be(cocktailResponse.Drinks!.First().Name);
                result.Instructions.Should().Be(cocktailResponse.Drinks!.First().Instructions);
                result.Glass.Should().Be(cocktailResponse.Drinks!.First().Glass);
                result.Thumbnail.Should().Be(cocktailResponse.Drinks!.First().Thumbnail);
                result.Ingredients!.First().Name.Should().Be(cocktailResponse.Drinks!.First().Ingredient1);
                result.Ingredients!.First().Measurement.Should().Be(cocktailResponse.Drinks!.First().Measure1);
                result.Ingredients!.Count.Should().Be(15);
            }

            [Theory, AutoData]
            public async Task GivenCallToGetRandomCocktail_ShouldThrowException(CocktailResponse cocktailResponse)
            {
                cocktailResponse.Drinks = null;
                _cocktailDbService.Setup(cds => cds!.GetRandomCocktail()).ReturnsAsync(cocktailResponse);

                Func<Task> subject = async () => await _cocktailProvider.GetRandomCocktail();

                await subject.Should().ThrowAsync<Exception>();
            }
        }
    }
}