using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Application;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
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
                Assert.Throws<ArgumentNullException>(() => new SmsController(null, Mock.Of<IUserProvider>(), Mock.Of<IViewProvider>()));
            }

            [Fact]
            public void GivenNullUserProvider_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SmsController(Mock.Of<ICocktailProvider>(), null, Mock.Of<IViewProvider>()));
            }

            [Fact]
            public void GivenNullViewProvider_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SmsController(Mock.Of<ICocktailProvider>(), Mock.Of<IUserProvider>(), null));
            }
        }

        public class GetCocktailRecipeTests
        {
            private readonly Mock<ICocktailProvider> _cocktailProvider = new();
            private readonly Mock<IUserProvider> _userProvider = new();
            private readonly Mock<IViewProvider> _viewProvider = new();
            private readonly SmsController _smsController;

            public GetCocktailRecipeTests()
            {
                _smsController = new SmsController(_cocktailProvider.Object, _userProvider.Object, _viewProvider.Object);
            }

            [Theory, AutoData]
            public async Task GivenSmsRequestWithCocktailName_ShouldGetCocktail(SmsRequest smsRequest, Cocktail cocktail, MessagingResponse messagingResponse)
            {
                _cocktailProvider.Setup(cp => cp.GetCocktail(It.IsAny<string>())).ReturnsAsync(cocktail);
                _viewProvider.Setup(vp => vp.GetResponseMessage()).Returns(messagingResponse);

                 var result = await _smsController.GetCocktailRecipe(smsRequest);

                _cocktailProvider.VerifyAll();
                _viewProvider.Verify(vp => vp.AddCocktailResponse(cocktail), Times.Once);
                result.Should().BeEquivalentTo(new TwiMLResult(messagingResponse));
            }

            [Theory, AutoData]
            public async Task GivenGetCocktailException_ShouldReturnErrorResponse(SmsRequest smsRequest)
            {
                _cocktailProvider.Setup(cp => cp.GetCocktail(It.IsAny<string>())).ThrowsAsync(new Exception());

                var result = await _smsController.GetCocktailRecipe(smsRequest);

                _viewProvider.Verify(vp => vp.AddErrorMessage("Unknown cocktail"));
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
                _viewProvider.Verify(vp => vp.AddCocktailResponse(cocktail), Times.Once);
               
            }

            [Theory, AutoData]
            public async Task GivenGetRandomCocktailException_ShouldReturnErrorResponse(SmsRequest smsRequest)
            {
                smsRequest.Body = "Random";
                _cocktailProvider.Setup(cp => cp.GetRandomCocktail()).ThrowsAsync(new Exception());

                var result = await _smsController.GetCocktailRecipe(smsRequest);

                _viewProvider.Verify(vp => vp.AddErrorMessage("Failed to get random cocktail"));
            }
        }
    }
}