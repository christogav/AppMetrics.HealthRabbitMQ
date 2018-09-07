namespace App.Metrics.Health.Checks.RabbitMQ
{
    using global::RabbitMQ.Client;
    using Health;

    /// <summary>
    /// Rabbit Health Check Extensions.
    /// </summary>
    public static class RabbitMQHealthCheckExtensions
    {

        /// <summary>
        /// Add a RabbitMQ HealthCheck.
        /// </summary>
        /// <param name="builder"><see cref="IHealthCheckBuilder"/></param>
        /// <param name="name">The name of the Health Check</param>
        /// <param name="connectionFactory">A RabbitMQ Connection Factory</param>
        /// <returns><see cref="IHealthBuilder"/></returns>
        public static IHealthBuilder AddRabbitMQHealthCheck(
            this IHealthCheckBuilder builder,
            string name,
            IConnectionFactory connectionFactory)
        {
            return builder.AddCheck(new RabbitMQHealthCheck(name, connectionFactory));
        }

        /// <summary>
        /// Add a RabbitMQ HealthCheck.
        /// </summary>
        /// <param name="builder"><see cref="IHealthCheckBuilder"/></param>
        /// <param name="name">The name of the Health Check</param>
        /// <param name="hostName"><see cref="ConnectionFactory.HostName"/></param>
        /// <param name="username"><see cref="ConnectionFactory.UserName"/></param>
        /// <param name="password"><see cref="ConnectionFactory.Password"/></param>
        /// <param name="virtualHost"><see cref="ConnectionFactory.VirtualHost"/></param>
        /// <returns><see cref="IHealthBuilder"/></returns>
        public static IHealthBuilder AddRabbitMQHealthCheck(
            this IHealthCheckBuilder builder,
            string name,
            string hostName,
            string username = null,
            string password = null,
            string virtualHost = null)
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = username,
                Password = password,
                VirtualHost = virtualHost
            };

            return builder.AddRabbitMQHealthCheck(name, connectionFactory);
        }
    }
}