using Refit;
using TVShows.Adapters.MazeApi.Models;

namespace TVShows.Adapters.MazeApi.Adapter
{
    public interface ITVMazeApi
    {
        [Get("/shows")]
        Task<IEnumerable<ShowResponse>> GetShowsAsync(int page, CancellationToken cancellationToken);

        [Get("/shows/{id}/cast")]
        Task<IEnumerable<CastResponse>> GetCastAsync(int id, CancellationToken cancellationToken);
    }
}
