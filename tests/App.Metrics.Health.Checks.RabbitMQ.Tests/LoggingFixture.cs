namespace App.Metrics.Health.Checks.RabbitMQ.Tests
{
    using System;
    using Logging;
    using Moq;

    public class LoggingFixture
    {
        public LoggingFixture()
        {
            var logProvider = new Mock<ILogProvider>(MockBehavior.Strict);
            logProvider.Setup(_ => _.GetLogger(It.IsAny<string>()))
                .Returns(Logger);
            LogProvider.SetCurrentLogProvider(logProvider.Object);
        }

        public bool Logger(LogLevel loglevel, Func<string> messagefunc, Exception exception, object[] formatparameters)
        {
            LogLevel = loglevel;
            MessageFunc = messagefunc;
            Exception = exception;
            Parameters = formatparameters;

            return true;
        }

        public LogLevel LogLevel = default(LogLevel);
        public Func<string> MessageFunc = null;
        public Exception Exception = null;
        public object[] Parameters = null;

        public void Clean()
        {
            LogLevel = default(LogLevel);
            MessageFunc = null;
            Exception = null;
            Parameters = null;
        }
    }
}