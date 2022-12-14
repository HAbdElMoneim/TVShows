
namespace TVShows.Application.ShowsUpdateData
{
    /// <summary>
    /// ITVShowsProvider
    /// </summary>
    public interface ITVShowsUpdateService
    {
        /// <summary>
        /// AddNewShowsAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddNewShowsAsync(CancellationToken cancellationToken);
    }
}
