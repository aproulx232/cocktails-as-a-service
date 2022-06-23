using Moq;
using Moq.Protected;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Test.Acceptance
{
    public static class MockHelper
    {
        public static void SetupHttpCall(Mock<HttpMessageHandler> mockHttpMessageHandler, string requestUrl, HttpResponseMessage httpResponseMessage)
        {
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == new Uri(requestUrl)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);
        }

        public static void SetupHttpCallWithCallback(Mock<HttpMessageHandler> mockHttpMessageHandler, string requestUri, HttpResponseMessage httpResponseMessage, Action<HttpRequestMessage, CancellationToken> callback)
        {
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(requestMessage => requestMessage.RequestUri == new Uri(requestUri)),
                    ItExpr.IsAny<CancellationToken>())
                .Callback(callback)
                .ReturnsAsync(httpResponseMessage);
        }

        public static void VerifyHttpCall(Mock<HttpMessageHandler> mockHttpMessageHandler, HttpMethod requestHttpMethod, string requestUrl, Times timesCalled)
        {
            mockHttpMessageHandler
                .Protected()
                .Verify("SendAsync",
                        timesCalled,
                        ItExpr.Is<HttpRequestMessage>(request => request.Method == requestHttpMethod &&
                                                                 request.RequestUri == new Uri(requestUrl)),
                        ItExpr.IsAny<CancellationToken>());
        }
    }
}
