using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace VendingApp.Console.Logging
{
    public class LoggerProxy : ILogger
    {
        private readonly ILogger _logger;

        public LoggerProxy(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public void Log(LogLevel logLevel, int eventId, object state,
            Exception exception, Func<object, Exception, string> formatter)
        {
            if (logLevel > LogLevel.Debug)
                logLevel = LogLevel.Debug;

            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel > LogLevel.Debug)
                logLevel = LogLevel.Debug;

            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel > LogLevel.Debug)
                logLevel = LogLevel.Debug;

            return _logger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }
    }

    public class LoggerFactoryProxy : ILoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public LoggerFactoryProxy(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _loggerFactory = loggerFactory;
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = _loggerFactory.CreateLogger(categoryName);

            if (categoryName.StartsWith("Microsoft.Data.Entity.", StringComparison.OrdinalIgnoreCase))
                logger = new LoggerProxy(logger);

            return logger;
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _loggerFactory.AddProvider(provider);
        }

        public void Dispose()
        {
            _loggerFactory.Dispose();
        }
    }
}
