namespace App.Metrics.Health.Checks.RabbitMQ.Tests
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Exceptions;
    using Xunit;

    public class RabbitMQHealthCheckTests : IClassFixture<LoggingFixture>
    {
        private readonly LoggingFixture _loggingFixture;

        public RabbitMQHealthCheckTests(LoggingFixture loggingFixture)
        {
            _loggingFixture = loggingFixture;
            _loggingFixture.Clean();
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

            var healthCheck = new RabbitMQHealthCheck("test", connectionFactory);

            // act
            var result = await healthCheck.ExecuteAsync().ConfigureAwait(false);

            // assert
            result.Should().NotBeNull()
                .And.Subject.As<HealthCheck.Result>().Check.Status.Should().Be(HealthCheckStatus.Unhealthy);
            _loggingFixture.Exception.Should().NotBeNull()
                .And.BeOfType<BrokerUnreachableException>();
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

            var healthCheck = new RabbitMQHealthCheck("test", connectionFactory);

            // act
            var result = await healthCheck.ExecuteAsync().ConfigureAwait(false);

            // assert
            result.Should().NotBeNull()
                .And.Subject.As<HealthCheck.Result>().Check.Status.Should().Be(HealthCheckStatus.Healthy);
        }
    }
}