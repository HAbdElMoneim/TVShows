using Microsoft.Extensions.Logging;
using TVShows.Caching;
using TVShows.Core.Extentions;
using TVShows.Core.Models;

namespace TVShows.Application.ShowsReadData
{
    public class TVShowsReadDataService : ITVShowsReadDataService
    {
        private readonly ITVShowsCacheService _tVShowsCacheService;
        private readonly ILogger<TVShowsReadDataService> _logger;

        public TVShowsReadDataService(ITVShowsCacheService tvShowsCacheService, ILogger<TVShowsReadDataService> logger)
        {
            _logger = logger;
            _tVShowsCacheService = tvShowsCacheService;
        }

        /// <summary>
        /// GetShowsDataAsync
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Show>> GetShowsDataAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                var showIds = await _tVShowsCacheService.GetShowIDs(cancellationToken);

                if (showIds is null)
                {
                    _logger.NoDataAvailable();


                    return new List<Show>();
                }
                var showsInfo = GetPaginatedShows(pageIndex, pageSize, showIds, cancellationToken);

                _logger.ShowsRetrievedSuccessfully(showsInfo.Count);

                return showsInfo;
            }
            catch (Exception ex)
            {
                _logger.FailedToGetShows(pageIndex, ex);

                throw;
            }
        }

        private List<Show> GetPaginatedShows(int pageIndex, int pageSize, int[] showIds, CancellationToken cancellationToken)
        {
            return showIds.Skip(pageIndex * pageSize)
                          .Take(pageSize)
                          .Select(async id => await _tVShowsCacheService.GetShowInfoWithCastInfo(id, cancellationToken))
                          .Select(task => task.Result)
                          .ToList();
        }
    }
}
