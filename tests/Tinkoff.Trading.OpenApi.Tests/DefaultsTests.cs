using System;
using FluentAssertions;
using Tinkoff.Trading.OpenApi.Network;
using Xunit;

namespace Tinkoff.Trading.OpenApi.Tests
{
    public class DefaultsTests
    {
        [Fact]
        public void DateTimeKindShouldThrowUnspecified()
        {
            new Defaults().Invoking(d => d.DateTimeKind = DateTimeKind.Unspecified)
                .Should().Throw<InvalidOperationException>()
                .Which.Message
                .Should().Be("Default DateTimeKind must be specified");
        }     
        
        [Theory]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Utc)]
        public void DateTimeKindShouldNotThrowSpecified(DateTimeKind dateTimeKind)
        {
            new Defaults().Invoking(d => d.DateTimeKind = dateTimeKind).Should().NotThrow();
        }

        [Fact]
        public void DefaultDateTimeKind()
        {
            new Defaults().DateTimeKind.Should().Be(DateTimeKind.Local);
        }
    }
}
