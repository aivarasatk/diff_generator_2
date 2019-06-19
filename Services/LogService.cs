using DiffGenerator2.Interfaces;
using Serilog;
using Serilog.Core;
using System;

namespace DiffGenerator2.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger _logger;
        public LogService(ILogger logger)
        {
            _logger = logger;
        }

        public void Information(string message)
        {
            _logger.Information(message);
        }

        public void Information(string messageTemplate, params object[] values)
        {
            _logger.Information(messageTemplate, values);
        }

        public void Information(string message, Exception ex)
        {
            _logger.Information(ex, message);
        }


        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string messageTemplate, params object[] values)
        {
            _logger.Error(messageTemplate, values);
        }

        public void Error(string message, Exception ex)
        {
            _logger.Error(ex, message);
        }

        
    }
}
