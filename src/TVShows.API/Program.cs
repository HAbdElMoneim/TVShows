using Polly;
using Polly.Extensions.Http;
using System.Net;
using TVShows.Caching;
using TVShowsUpdateWorkerJob;
using TVShowsUpdateWorkerJob.Configurations;
using TVShows.Application.ShowsUpdateData;
using TVShows.Application.ShowsReadData;
using TVShows.Adapters.MazeApi.Configurations;
using TVShows.Application.Ports;
using TVShows.Adapters.Port;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var tvMazeSettings = builder.Configuration.GetSection("TVMazeSettings").Get<TVMazeSettings>();

builder.Services.AddTVMazeAdapter(tvMazeSettings).AddPolicyHandler(HttpRetryPolicy());

builder.Services.AddTransient<ITVShowsUpdateService, TVShowsUpdateService>();
builder.Services.AddTransient<ITVMazeService, TVMazeService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<ITVShowsReadDataService, TVShowsReadDataService>();
builder.Services.AddTransient<ITVShowsCacheService, TVShowsCacheService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Configuration.GetValue<bool>("SchedulingConfigurations:Enabled"))
{
    builder.Services.AddSingleton(BuildWorkerJobConfiguration());
    ScheduleConfigurations BuildWorkerJobConfiguration()
    {
        ScheduleConfigurations scheduleConfigurations = new();
        scheduleConfigurations.Schedule = builder.Configuration["SchedulingConfigurations:Schedule"];
        return scheduleConfigurations;
    }
    builder.Services.AddHostedService<TVShowsUpdateJob>();
}

IAsyncPolicy<HttpResponseMessage> HttpRetryPolicy() => HttpPolicyExtensions
                   .HandleTransientHttpError()
                   .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                   .WaitAndRetryAsync(int.Parse(builder.Configuration["RetryAttempt"]), retryAttempt => TimeSpan.FromSeconds(Math.Pow(int.Parse(builder.Configuration["RetryBaseTimeSec"]), retryAttempt)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
