namespace App.Metrics.Health.Checks.RabbitMQ.Tests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Exceptions;
    using Logging;
    using Moq;
    using Xunit;

    public class RabbitMQHealthCheckTests
    {
        private readonly Mock<ILog> _log;

        public RabbitMQHealthCheckTests()
        {
            _log = new Mock<ILog>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckAsync_WhenFaultyConnectionDetails_ReturnsUnHealthyResult()
        {
            // arrange
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5673 // wrong
            };

            _log.Setup(_ => _.Log(LogLevel.Error, It.IsAny<Func<string>>(), It.IsAny<BrokerUnreachableException>(), It.IsAny<object[]>()))
                .Returns(true)
                .Verifiable();

            var healthCheck = new RabbitMQHealthCheck("test", connectionFactory, _log.Object);

            // act
            var result = await healthCheck.ExecuteAsync().ConfigureAwait(false);

            // assert
            result.Should().NotBeNull()
                .And.Subject.As<HealthCheck.Result>().Check.Status.Should().Be(HealthCheckStatus.Unhealthy);
            _log.Verify();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CheckAsync_WhenCheckPasses_ReturnsHealthyResult()
        {
            // arrange
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            _log.Setup(_ => _.Log(LogLevel.Error, It.IsAny<Func<string>>(), It.IsAny<Exception>(), It.IsAny<object[]>()))
                .Returns(true);

            var healthCheck = new RabbitMQHealthCheck("test", connectionFactory, _log.Object);

            // act
            var result = await healthCheck.ExecuteAsync().ConfigureAwait(false);

            // assert
            result.Should().NotBeNull()
                .And.Subject.As<HealthCheck.Result>().Check.Status.Should().Be(HealthCheckStatus.Healthy);
        }
    }
}