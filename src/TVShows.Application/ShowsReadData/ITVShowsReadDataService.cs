using TVShows.Core.Models;

namespace TVShows.Application.ShowsReadData
{
    public interface ITVShowsReadDataService
    {
        Task<IReadOnlyCollection<Show>> GetShowsDataAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
    }
}
