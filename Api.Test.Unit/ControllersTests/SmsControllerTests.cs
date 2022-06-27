using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Application;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Twilio.AspNet.Common;
using Xunit;

namespace Api.Test.Unit.ControllersTests
{
    public class SmsControllerTests
    {
        public class ConstructorTests
        {
            [Fact]
            public void GivenNullCocktailProvider_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SmsController(null));
            }
        }

        public class GetCocktailRecipeTests
        {
            private readonly Mock<ICocktailProvider> _cocktailProvider = new();
            private readonly SmsController _smsController;

            public GetCocktailRecipeTests()
            {
                _smsController = new SmsController(_cocktailProvider.Object);
            }

            [Theory, AutoData]
            public async Task GivenSmsRequestWithCocktailName_ShouldGetCocktail(SmsRequest smsRequest, Cocktail cocktail)
            {
                _cocktailProvider.Setup(cp => cp.GetCocktail(It.IsAny<string>())).ReturnsAsync(cocktail);

                 var result = await _smsController.GetCocktailRecipe(smsRequest);

                _cocktailProvider.VerifyAll();
                result.Data.Should().Contain(cocktail.Name);
                result.Data.Should().Contain(cocktail.Instructions);
                result.Data.Should().Contain(cocktail.Ingredients!.First().Name);
                result.Data.Should().Contain(cocktail.Ingredients!.First().Name);
            }

            [Theory, AutoData]
            public async Task GivenGetCocktailException_ShouldReturnErrorResponse(SmsRequest smsRequest)
            {
                _cocktailProvider.Setup(cp => cp.GetCocktail(It.IsAny<string>())).ThrowsAsync(new Exception());

                var result = await _smsController.GetCocktailRecipe(smsRequest);

                result.Data.Should().Contain("Unknown cocktail");
            }

            [Theory]
            [InlineAutoData("Random")]
            [InlineAutoData("random")]
            [InlineAutoData("RANDOM")]
            [InlineAutoData(" Random ")]
            public async Task GivenSmsRequestWithRandom_ShouldGetCocktail(string randomRequestBody, SmsRequest smsRequest, Cocktail cocktail)
            {
                smsRequest.Body = randomRequestBody;
                _cocktailProvider.Setup(cp => cp.GetRandomCocktail()).ReturnsAsync(cocktail);

                var result = await _smsController.GetCocktailRecipe(smsRequest);

                _cocktailProvider.VerifyAll();
                result.Data.Should().Contain(cocktail.Name);
                result.Data.Should().Contain(cocktail.Instructions);
                result.Data.Should().Contain(cocktail.Ingredients!.First().Name);
                result.Data.Should().Contain(cocktail.Ingredients!.First().Name);
            }

            [Theory, AutoData]
            public async Task GivenGetRandomCocktailException_ShouldReturnErrorResponse(SmsRequest smsRequest)
            {
                smsRequest.Body = "Random";
                _cocktailProvider.Setup(cp => cp.GetRandomCocktail()).ThrowsAsync(new Exception());

                var result = await _smsController.GetCocktailRecipe(smsRequest);

                result.Data.Should().Contain("Failed to get random cocktail");
            }
        }
    }
}