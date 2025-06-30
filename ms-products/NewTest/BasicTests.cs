using NUnit.Framework;
using FluentAssertions;
using Moq;
using Products.Interfaces;
using System.Threading.Tasks;

namespace NewTest
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void NUnitTest_ShouldPass()
        {
            Assert.That(1 + 1, Is.EqualTo(2));
        }

        [Test]
        public void FluentAssertions_ShouldWork()
        {
            var result = "Hello World";
            result.Should().Contain("Hello");
        }

        [Test]
        public async Task Moq_ShouldWork()
        {
            var mock = new Mock<ICurrencyConversionService>();
            mock.Setup(m => m.ConvertAsync(10m, "USD", "EUR")).ReturnsAsync(8.5m);
            
            var result = await mock.Object.ConvertAsync(10m, "USD", "EUR");
            
            result.Should().Be(8.5m);
        }
    }
}
