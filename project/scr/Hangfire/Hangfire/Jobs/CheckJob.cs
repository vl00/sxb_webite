using System;
using System.Linq;
using System.Threading;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace Hangfire.ConsoleWeb.Jobs
{
    public interface ICheckJob
    {
        void Check(PerformContext context);
    }
    public class CheckJob : ICheckJob
    {
        private readonly ILogger<CheckJob> _logger;
        private ITimerJob _timeJob;
        public CheckJob(ILoggerFactory loggerFactory,
            ITimerJob timerJob)
        {
            _logger = loggerFactory.CreateLogger<CheckJob>();
            _timeJob = timerJob;
        }

        public void Check(PerformContext context)
        {
            _logger.LogInformation($"check service start checking, now is {DateTime.Now}");
            context.WriteLine($"check service start checking, now is {DateTime.Now}");
            BackgroundJob.Schedule(() => _timeJob.Timer(null), TimeSpan.FromMilliseconds(30));

            var progressBar = context.WriteProgressBar();

            foreach (var i in Enumerable.Range(1, 50).ToList().WithProgress(progressBar))
            {

                Thread.Sleep(1000);
            }
            _logger.LogInformation($"check is end, now is {DateTime.Now}");
            context.WriteLine($"check is end, now is {DateTime.Now}");
        }
    }
}
