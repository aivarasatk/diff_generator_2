using DiffGenerator2.Interfaces;
using Serilog;
using Serilog.Core;

namespace DiffGenerator2.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger _logger;
        public LogService(ILogger logger)
        {
            _logger = logger;
        }
    }
}
