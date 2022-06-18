using System.Net;

namespace Test.Shared
{
    public class MockHttpClient
    {
        private readonly MockMessageHandler _messageHandler;

        private readonly IDictionary<string, (HttpStatusCode code, StringContent content)> _returnContent = new Dictionary<string, (HttpStatusCode code, StringContent content)>();

        public bool UseRequestUriAsPath { get; set; }

        public MockHttpClient()
        {
            _messageHandler = new MockMessageHandler(this);
        }

        public void Returns(string path, HttpStatusCode statusCode, string content = "")
        {
            _returnContent.Add(path, (statusCode, new StringContent(content)));
        }

        public IEnumerable<HttpRequestMessage> RequestsFromPath(string path) =>
            _messageHandler.ReceivedRequests[path];

        private class MockMessageHandler : HttpMessageHandler
        {
            public readonly IDictionary<string, IList<HttpRequestMessage>> ReceivedRequests = new Dictionary<string, IList<HttpRequestMessage>>();

            private readonly MockHttpClient _parent;

            public MockMessageHandler(MockHttpClient parent)
            {
                _parent = parent;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var path = _parent.UseRequestUriAsPath ? request.RequestUri!.AbsoluteUri : request.RequestUri!.AbsolutePath;

                if (ReceivedRequests.ContainsKey(path))
                {
                    ReceivedRequests[path].Add(request);
                }
                else
                {
                    ReceivedRequests.Add(path, new List<HttpRequestMessage> { request });
                }

                if (_parent._returnContent.ContainsKey(path))
                {
                    var (code, content) = _parent._returnContent[path];
                    return await Task.FromResult(new HttpResponseMessage(code)
                    {
                        Content = content
                    });
                }

                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }

        public HttpClient CreateClient() => new HttpClient(_messageHandler) { BaseAddress = new Uri("https://fake.url") };
    }
}