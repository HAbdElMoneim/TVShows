using TVShows.Core.Models;

namespace TVShows.API.Services
{
    public interface IShowsDataService
    {
        Task<IReadOnlyCollection<Show>> GetShowsDataAsync(int pageIndex, int pageSize, CancellationToken cancellationToken);
    }
}
