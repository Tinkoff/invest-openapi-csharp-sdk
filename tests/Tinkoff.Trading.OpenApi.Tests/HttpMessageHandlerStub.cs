using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tinkoff.Trading.OpenApi.Tests
{
    class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly HttpResponseMessage _responseMessage;
        
        public HttpRequestMessage RequestMessage { get; private set; }

        public HttpMessageHandlerStub(HttpResponseMessage responseMessage)
        {
            _responseMessage = responseMessage;
        }

        public HttpMessageHandlerStub(HttpStatusCode code, string content)
            : this(new HttpResponseMessage(code) {Content = new StringContent(content)})
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestMessage = request;
            return Task.FromResult(_responseMessage);
        }
    }
}