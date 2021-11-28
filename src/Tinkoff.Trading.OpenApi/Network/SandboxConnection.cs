using System.Net.Http;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class SandboxConnection : Connection<SandboxContext>
    {
        public SandboxConnection(string token, HttpClient httpClient)
            : base("https://api-invest.tinkoff.ru/openapi/sandbox/", token, httpClient)
        {
        }

        public SandboxConnection(string baseUri, string webSocketBaseUri, string token, HttpClient httpClient)
            : base(baseUri, webSocketBaseUri, token, httpClient)
        {
        }

        public override SandboxContext Context => new SandboxContext(this);
    }
}