using TVShows.Core.Models;

namespace TVShows.Caching
{
    public interface ITVShowsCacheService
    {
        Task<int[]> GetShowIDs(CancellationToken cancellationToken);
        Task<Show> GetShowInfoWithCastInfo(int showId, CancellationToken cancellationToken);
        Task AddShowIds(List<int> showIds, CancellationToken cancellationToken);
        Task AddShowWithCasts(Show show, CancellationToken cancellationToken);
    }
}