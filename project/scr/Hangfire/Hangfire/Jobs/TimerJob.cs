using System;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace Hangfire.ConsoleWeb.Jobs
{
    public interface ITimerJob
    {
        void Timer(PerformContext context);
    }
    public class TimerJob : ITimerJob
    {
        private readonly ILogger<TimerJob> _logger;
        public TimerJob(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerJob>();
        }
        public void Timer(PerformContext context)
        {
            _logger.LogInformation($"timer service is starting, now is {DateTime.Now}");
            context.WriteLine($"timer service is starting, now is {DateTime.Now}");

            _logger.LogWarning("timering");
            context.WriteLine("timering");

            _logger.LogInformation($"timer is end, now is {DateTime.Now}");
            context.WriteLine($"timer is end, now is {DateTime.Now}");
        }
    }
}
