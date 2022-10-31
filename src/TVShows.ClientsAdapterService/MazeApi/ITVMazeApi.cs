using Refit;
using TVShowsClientsAdapterService.MazeApi.Models;

namespace TVShowsClientsAdapterService.MazeApi
{
    public interface ITVMazeApi
    {
        [Get("/shows")]
        Task<IEnumerable<ShowResponse>> GetShowsAsync(int page, CancellationToken cancellationToken);

        [Get("/shows/{id}/cast")]
        Task<IEnumerable<CastResponse>> GetCastAsync(int id, CancellationToken cancellationToken);
    }
}
