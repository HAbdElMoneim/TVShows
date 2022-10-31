using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using TVShows.Core.Extentions;
using TVShows.Core.Models;
using TVShowsClientsAdapterService.MazeApi.Models;
using TVShowsUpdateWorkerJob.Configurations;
using TVShowsUpdateWorkerJob.Services;

namespace TVShowsUpdateWorkerJob
{
    public sealed class TVShowsUpdateJob : BackgroundService
    {
        private readonly ScheduleConfigurations _scheduleConfigurations;
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;

        private readonly ITVShowsUpdateService _tvShowsUpdateService;
        private readonly ILogger<TVShowsUpdateJob> _logger;

        public TVShowsUpdateJob(ILogger<TVShowsUpdateJob> logger, ITVShowsUpdateService tvShowsUpdateService, ScheduleConfigurations scheduleConfigurations)
        {
            _scheduleConfigurations = scheduleConfigurations;
            _tvShowsUpdateService = tvShowsUpdateService;
            _logger = logger;
            _schedule = CrontabSchedule.Parse(_scheduleConfigurations.Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var myList = new List<ShowResponse>();
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            cancellationToken.Register(() => _logger.ServiceStopped(typeof(TVShowsUpdateJob).Name));
            do
            {
                var now = DateTime.Now;
                if (DateTime.Compare(now, _nextRun) == 1)
                {
                    try
                    {
                        _logger.ShowsUpdatesStarted(typeof(TVShowsUpdateJob).Name);
                        await _tvShowsUpdateService.AddNewShowsAsync(cancellationToken);
                        _logger.LogInformation("Worker End at: {time}", DateTimeOffset.Now);

                        _nextRun = _schedule.GetNextOccurrence(now);
                        await Task.Delay(_nextRun - DateTime.Now, cancellationToken);
                    }                    
                    catch (Exception ex)
                    {
                        _logger.ServiceFailed(ex);
                        await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                    }
                }
            }
            while (!cancellationToken.IsCancellationRequested);
        }
    }
}
