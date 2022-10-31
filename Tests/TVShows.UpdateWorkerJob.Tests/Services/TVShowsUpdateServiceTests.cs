using AutoFixture.Xunit2;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TVShows.Caching;
using TVShows.Core.Models;
using TVShowsClientsAdapterService.MazeApi;
using TVShowsUpdateWorkerJob.Services;
using Xunit;

namespace TVShows.UpdateWorkerJob.Tests.Services
{
    public class TVShowsUpdateServiceTests
    {
        private readonly TVShowsUpdateService _tvShowsUpdateService;
        private readonly Mock<ITVMazeService> _tvMazeServiceMock;
        private readonly Mock<ITVShowsCacheService> _tvShowsCacheServiceMock;
        private readonly Mock<ILogger<TVShowsUpdateService>> _loggerMock;

        public TVShowsUpdateServiceTests()
        {
            _tvMazeServiceMock = new Mock<ITVMazeService>();
            _tvShowsCacheServiceMock = new Mock<ITVShowsCacheService>();
            _loggerMock = new Mock<ILogger<TVShowsUpdateService>>();
            _tvShowsUpdateService = new TVShowsUpdateService(_tvShowsCacheServiceMock.Object, _tvMazeServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        internal async Task GivenUseCase_WhenAddNewShowsAsync_WithNoShowInfoExistAndNoCastFilled()
        {
            // Arrange
            _tvMazeServiceMock.Setup(x => x.GetShowsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetShowsInfo()));
            _tvMazeServiceMock.Setup(x => x.GetCastAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetCastInfo()));

            _tvShowsCacheServiceMock.Setup(x => x.AddShowIds(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()));
            _tvShowsCacheServiceMock.Setup(x => x.AddShowWithCasts(It.IsAny<Show>(), It.IsAny<CancellationToken>()));

            // Action
            await _tvShowsUpdateService.AddNewShowsAsync(default);

            // Assert
            _tvShowsCacheServiceMock.Verify(x => x.AddShowIds(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()), Times.Never);
            _tvShowsCacheServiceMock.Verify(x => x.AddShowWithCasts(It.IsAny<Show>(), It.IsAny<CancellationToken>()), Times.Never);
            _tvMazeServiceMock.Verify(x => x.GetCastAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private static IEnumerable<Show> GetShowsInfo() => new List<Show>()
        {
           
        };
        
        private static IEnumerable<Cast> GetCastInfo() => new List<Cast>()
        {
            new Cast()
            {
                Person = new Person()
                {
                    Birthday = "06-05-1978",
                    Id = 1,
                    Name = "Hany"
                }
            },
            new Cast()
            {
                 Person = new Person()
                {
                    Birthday = "11-11-1980",
                    Id = 2,
                    Name = "Aly"
                }
            }
        };

    }
}
