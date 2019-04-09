using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Services
{
    public class LoggerBuilder
    {
        public static ILogger Build()
        {
            return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("Logs/logs.txt",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
            .CreateLogger();
        }
    }
}
