using Microsoft.Extensions.Logging;

namespace TVShows.Core.Extentions
{
    public static class Logger
    {
        public static void ShowsNotFound(this ILogger logger, int pageIndex) =>
            logger.LogWarning($"Shows are not returned from TvMaze API for page index: {pageIndex}");

        public static void CastsNotFound(this ILogger logger, int showId) =>
            logger.LogWarning($"Casts are not returned from TvMaze API for show Id: {showId}");

        public static void NoDataAvailable(this ILogger logger) =>
           logger.LogWarning("Data for show Ids is not availableat the moment..");

        public static void ShowsRetrievedSuccessfully(this ILogger logger, int showCount) =>
           logger.LogInformation($"Shows with their casts retrieved successfully: {showCount} Show...");

        public static void FailedToGetShows(this ILogger logger, int pageIndex, Exception exception) =>
            logger.LogError($"Failed to get shows for page index {pageIndex}. error: {exception}");

        public static void ServiceStarted(this ILogger logger, string serviceName) =>
    logger.LogInformation($"Started {serviceName} at {DateTime.UtcNow}.");

        public static void ServiceStopped(this ILogger logger, string serviceName) =>
            logger.LogInformation($"Stopped {serviceName} at {DateTime.UtcNow}.");

        public static void ServiceFailed(this ILogger logger, Exception exception) =>
            logger.LogError($"Scheduled Service failed due to {exception.Message}. Inner exception: {exception.InnerException}");

        public static void ShowsUpdatesStarted(this ILogger logger, string serviceName) =>
           logger.LogError($"Starting Service {serviceName} for scrapping and storing the shows along with casts.");

        public static void ShowsUpdatesFailed(this ILogger logger, Exception exception) =>
            logger.LogError($"Scrapping failed due to {exception.Message}. Inner exception: {exception.InnerException}");

        public static void CacheIsNotAvailableForShowIds(this ILogger logger) =>
            logger.LogWarning("Data for show Ids is not available in cache");

        public static void CastInfoIsNotAvailable(this ILogger logger, int showId) =>
            logger.LogWarning($"Cast information is not yet available for that showId: {showId}");
    }
}
