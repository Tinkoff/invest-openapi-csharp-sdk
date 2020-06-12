using System.Net.Http;
using FluentAssertions;
using Tinkoff.Trading.OpenApi.Network;
using Xunit;

namespace Tinkoff.Trading.OpenApi.Tests
{
    public class ConnectionTests
    {
        private class FakeConnection : Connection
        {
            public FakeConnection() : base("http://localhost", "http://localhost", "", new HttpClient())
            {
            }
        }

        [Fact]
        public void ShouldInitializeDefaults()
        {
            new FakeConnection().Defaults.Should().BeEquivalentTo(new Defaults());
        }
    }
}
