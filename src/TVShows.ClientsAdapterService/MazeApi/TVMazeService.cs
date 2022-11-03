using Microsoft.Extensions.Logging;
using Refit;
using TVShows.Core.Extentions;
using TVShows.Core.Models;

namespace TVShowsClientsAdapterService.MazeApi
{
    public class TVMazeService : ITVMazeService
    {
        private readonly ITVMazeApi _tvMazeApi;
        private readonly ILogger<TVMazeService> _logger;

        public TVMazeService(ITVMazeApi tvMazeApi, ILogger<TVMazeService> logger)
        {
            _tvMazeApi = tvMazeApi ?? throw new ArgumentNullException(nameof(tvMazeApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// GetShowsAsync
        /// </summary>
        /// <param name="page"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Show>> GetShowsAsync(int page, CancellationToken cancellationToken)
        {
            try
            {
                var shows = await _tvMazeApi.GetShowsAsync(page, cancellationToken);

                if (shows is null || !shows.Any())
                {
                    _logger.ShowsNotFound(page);

                    return null;
                };

                return shows.Select(s => s.ToDomain());
            }
            catch (ApiException ex)
            {
                _logger.ShowsUpdatesFailed(ex);
                throw;
            }
        }

        /// <summary>
        /// GetCastAsync
        /// </summary>
        /// <param name="showId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Cast>> GetCastAsync(int showId, CancellationToken cancellationToken)
        {
            try
            {
                var casts = await _tvMazeApi.GetCastAsync(showId, cancellationToken);

                if (casts is null || !casts.Any())
                {
                    _logger.CastsNotFound(showId);

                    return null;
                };

                return casts.OrderByDescending(x => x.Person.Birthday).Select(cast => cast.ToDomain());
            }
            catch (ApiException ex)
            {
                _logger.ShowsUpdatesFailed(ex);
                throw ;
            }
        }
    }
}
