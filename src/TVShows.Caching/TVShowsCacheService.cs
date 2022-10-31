using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TVShows.Core.Extentions;
using TVShows.Core.Models;

namespace TVShows.Caching
{
    // TODO: need more abstractions layer
    public class TVShowsCacheService : ITVShowsCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<TVShowsCacheService> _logger;

        /// <summary>
        /// TVShowsCacheService
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public TVShowsCacheService(IDistributedCache cache, ILogger<TVShowsCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// GetShowIDs
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int[]> GetShowIDs(CancellationToken cancellationToken)
        {
            var showIds = await _cache.GetStringAsync(CacheKeys.CacheKeyForShow, cancellationToken);

            if (string.IsNullOrWhiteSpace(showIds))
            {
                _logger.CacheIsNotAvailableForShowIds();

                return null;
            }

            return JsonSerializer.Deserialize<int[]>(showIds);           
        }

        /// <summary>
        /// GetShowInfoWithCastInfo
        /// </summary>
        /// <param name="showId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Show> GetShowInfoWithCastInfo(int showId, CancellationToken cancellationToken)
        {
            var castInfo = await _cache.GetStringAsync(CacheKeys.GetCacheKeyForShow(showId), cancellationToken);

            if (string.IsNullOrWhiteSpace(castInfo))
            {
                _logger.CastInfoIsNotAvailable(showId);

                return new Show { Id = showId , Name = "No more info is available yet..." };
            }

            return JsonSerializer.Deserialize<Show>(castInfo);
        }

        /// <summary>
        /// AddShowInfo
        /// </summary>
        /// <param name="showIds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddShowIds(List<int> showIds, CancellationToken cancellationToken)
        {
            await _cache.SetStringAsync(CacheKeys.CacheKeyForShow, JsonSerializer.Serialize(showIds.Select(s => s).ToArray()), cancellationToken);
        }

        /// <summary>
        /// AddShowWithCasts
        /// </summary>
        /// <param name="show"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddShowWithCasts(Show show, CancellationToken cancellationToken)
        {
            await _cache.SetStringAsync(CacheKeys.GetCacheKeyForShow(show.Id), JsonSerializer.Serialize(show), cancellationToken);
        }
    }
}