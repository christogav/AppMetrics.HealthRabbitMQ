using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("App.Metrics.Health.Checks.RabbitMQ.Tests")]

namespace App.Metrics.Health.Checks.RabbitMQ
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using global::RabbitMQ.Client;
    using Logging;

    internal class RabbitMQHealthCheck : HealthCheck
    {
        private const string MessageBody = "Hello, world!";
        private static readonly ILog Logger = LogProvider.For<IRunHealthChecks>();
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQHealthCheck(string name, IConnectionFactory connectionFactory)
            : base(name)
        {
            _connectionFactory = connectionFactory;
        }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var props = channel.CreateBasicProperties();
                    var routingKey = Guid.NewGuid().ToString();

                    byte[] messageBodyBytes = Encoding.UTF8.GetBytes(MessageBody);
                    channel.BasicPublish(string.Empty, routingKey, props, messageBodyBytes);

                    channel.Close();
                    connection.Close();
                }

                return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy($"OK. {Name} is available."));
            }
            catch (Exception ex)
            {
                Logger.ErrorException($"{Name} failed.", ex);

                return new ValueTask<HealthCheckResult>(
                    HealthCheckResult.Unhealthy($"Failed. {Name} is unavailable. ({ex.Message})"));
            }
        }
    }
}