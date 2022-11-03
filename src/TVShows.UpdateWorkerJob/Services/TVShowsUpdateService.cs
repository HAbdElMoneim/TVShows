using Microsoft.Extensions.Logging;
using TVShows.Caching;
using TVShows.Core.Extentions;
using TVShows.Core.Models;
using TVShowsClientsAdapterService.MazeApi;

namespace TVShowsUpdateWorkerJob.Services
{
    public class TVShowsUpdateService : ITVShowsUpdateService
    {
        private const int ShowBatches = 10; // TODO:Read from Configurations.
        private readonly ITVMazeService _tvMazeService;
        private readonly ITVShowsCacheService _tvShowsCacheService;
        private readonly ILogger<TVShowsUpdateService> _logger;

        /// <summary>
        /// TVShowsUpdateService
        /// </summary>
        /// <param name="tvShowsCacheService"></param>
        /// <param name="tvMazeService"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TVShowsUpdateService(
            ITVShowsCacheService tvShowsCacheService,
            ITVMazeService tvMazeService,
            ILogger<TVShowsUpdateService> logger)
        {
            _tvMazeService = tvMazeService ?? throw new ArgumentNullException(nameof(tvMazeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tvShowsCacheService = tvShowsCacheService;
        }

        /// <summary>
        /// AddNewShowsAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddNewShowsAsync(CancellationToken cancellationToken)
        {
            var showsList = new List<Show>();
            var pageIndex = 0; // TODO: read from cache;
            var isShowsInfoAvailable = true;

            try
            {
                while (isShowsInfoAvailable)
                {
                    // TODO: Need to get cast missed data from stored shows ids first[another function].

                    var response = await _tvMazeService.GetShowsAsync(pageIndex, cancellationToken);

                    if (response is null || !response.Any())
                    {
                        isShowsInfoAvailable = false;
                        continue;
                    }

                    showsList.AddRange(response.Select(x => x).ToList());

                    var showIds = GetShowIds(showsList);

                    await _tvShowsCacheService.AddShowIds(showIds, cancellationToken);
                                       
                    var batches = GetShowBatches(showsList, ShowBatches);

                    foreach (var batch in batches)
                    {
                        var processShowTasks = batch.Select(x => FillShowsWithCasts(x, cancellationToken));
                        await Task.WhenAll(processShowTasks);
                    }

                    pageIndex++;
                }

                // TODO: Add last pageIndex value to cache 
            }
            catch (Exception ex)
            {
                _logger.ShowsUpdatesFailed(ex);
                throw;
            }
        }

        private static List<int> GetShowIds(List<Show> shows)
        {
            return shows
                .Select(s => (int)s.Id)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        private async Task FillShowsWithCasts(Show show, CancellationToken cancellationToken)
        {
            var getCastResponse = await _tvMazeService.GetCastAsync(show.Id, cancellationToken);            

            if (getCastResponse is null)
            {
                return;
            }

            var showsWithCast = GetShowsWithCasts(show, getCastResponse);

            await _tvShowsCacheService.AddShowWithCasts(showsWithCast, cancellationToken);
        }

        private static Show GetShowsWithCasts(Show show, IEnumerable<Cast> getCastResponse) =>
            new()
            {
                Id = show.Id,
                Name = show.Name,
                Cast = getCastResponse
                        .Select(cast =>
                            new Cast
                            {
                                Person = new Person
                                {
                                    Id = cast.Person.Id,
                                    Name = cast.Person.Name,
                                    Birthday = cast.Person.Birthday,
                                }
                            })
                        .ToList()
            };

        private static IEnumerable<IEnumerable<Show>> GetShowBatches(ICollection<Show> shows, int batchSize)
        {
            var total = 0;

            while (total < shows.Count)
            {
                yield return shows.Skip(total).Take(batchSize);

                total += batchSize;
            }
        }
    }
}
