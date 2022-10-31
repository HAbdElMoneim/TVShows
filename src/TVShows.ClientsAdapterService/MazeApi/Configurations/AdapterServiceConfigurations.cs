using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace TVShowsClientsAdapterService.MazeApi.Configurations
{
    public static class AdapterServiceConfigurations
    {
        public static IHttpClientBuilder AddTVMazeAdapter(this IServiceCollection serviceCollection, TVMazeSettings tvMazeSettings)
        {
            _ = tvMazeSettings ?? throw new ArgumentNullException(nameof(tvMazeSettings));

            serviceCollection.AddSingleton(tvMazeSettings);

            serviceCollection.AddScoped<ITVMazeService, TVMazeService>();

            return serviceCollection.AddRefitClient<ITVMazeApi>().ConfigureTvMazeApi(tvMazeSettings);
        }

        private static IHttpClientBuilder ConfigureTvMazeApi(this IHttpClientBuilder hcb, TVMazeSettings settings)
            => hcb.ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.ApiUrl));
    }
}
