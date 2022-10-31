using TVShows.Core.Models;

namespace TVShowsClientsAdapterService.MazeApi
{
    public interface ITVMazeService
    {
        /// <summary>
        /// Get shows from TvMaze API on basis of page index
        /// </summary>
        /// <exception cref="GetShowsExceptions" />
        Task<IEnumerable<Show>> GetShowsAsync(int page, CancellationToken cancellationToken);

        /// <summary>
        /// Get casts from TvMaze API for a particular show
        /// </summary>
        /// <exception cref="GetCastsExceptions" />
        Task<IEnumerable<Cast>> GetCastAsync(int showId, CancellationToken cancellationToken);
    }
}
