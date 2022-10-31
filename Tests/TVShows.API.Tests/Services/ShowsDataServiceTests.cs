using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TVShows.Caching;
using TVShows.Core.Models;
using TVShows.API.Services;
using Xunit;

namespace TVShows.API.Tests.Services
{
    public class ShowsDataServiceTests
    {
        private readonly ShowsDataService _showsDataService;
        private readonly Mock<ITVShowsCacheService> _tvShowsCacheServiceMock;
        private readonly Mock<ILogger<ShowsDataService>> _loggerMock;

        public ShowsDataServiceTests()
        {
            _tvShowsCacheServiceMock = new Mock<ITVShowsCacheService>();
            _loggerMock = new Mock<ILogger<ShowsDataService>>();

            _showsDataService = new ShowsDataService(_tvShowsCacheServiceMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(0, 1)]
        internal async Task GivenUseCase_WhenGetShowsDataAsync_WithPageIndexAndPageSize_ShowCountIsEquivalentToSize(int pageIndex, int pageSize)
        {
            // Arrange
            _tvShowsCacheServiceMock.Setup(x => x.GetShowIDs(It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetCachedShowsIndex()));
            _tvShowsCacheServiceMock.Setup(x => x.GetShowInfoWithCastInfo(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetShowsWithCastFromCache()));

            // Action
            var shows = await _showsDataService.GetShowsDataAsync(pageIndex, pageSize, default);

            // Assert
            shows.Count.Should().Be(pageSize);
        }

        [Fact]
        internal async Task GivenUseCase_WhenGetShowsDataAsync_WithCacheReturnsEmptyResults_EmptyCollectionIsReturned()
        {
            // Arrange
            _tvShowsCacheServiceMock.Setup(x => x.GetShowIDs(It.IsAny<CancellationToken>())).Returns(Task.FromResult(new int[0]));
            _tvShowsCacheServiceMock.Setup(x => x.GetShowInfoWithCastInfo(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Show()));

            // Action
            var shows = await _showsDataService.GetShowsDataAsync(0, 1, default);

            // Assert
            _tvShowsCacheServiceMock.Verify(x => x.GetShowInfoWithCastInfo(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            shows.Count.Should().Be(0);
        }

        [Fact]
        internal async Task GivenUseCase_WhenGetShowsDataAsync_WithCastCacheIsNotStored_ShowIsReturnedWithoutCast()
        {
            // Arrange
            _tvShowsCacheServiceMock.Setup(x => x.GetShowIDs(It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetCachedShowsIndex()));
            _tvShowsCacheServiceMock.Setup(x => x.GetShowInfoWithCastInfo(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Show { Id = 1, Name = "No more info is available yet..." }));

            // Action
            var shows = await _showsDataService.GetShowsDataAsync(0, 1, default);

            // Assert
            shows.Should().NotBeNull();
            shows.Count.Should().Be(1);
            shows.FirstOrDefault()?.Cast.Should().BeEmpty();
            shows.FirstOrDefault()?.Name.Should().BeEquivalentTo("No more info is available yet...");
        }

        private static int[] GetCachedShowsIndex() => new int []{ 1, 2};
       

        private static Show GetShowsWithCastFromCache()
        {
            var show = new Show()
            {
                Id = 2,
                Name = "test",
                Cast = new Cast[]
                {
                    new Cast
                    {
                        Person = new Person
                        {
                            Id = 1,
                            Name = "person_1",
                            Birthday = DateTime.Now.AddDays(-10).ToString(),
                        }
                    },
                    new Cast
                    {
                        Person = new Person
                        {
                            Id = 2,
                            Name = "person_2",
                            Birthday = DateTime.Now.AddDays(-20).ToString(),
                        }
                    },
                }
            };

            return show;
        }
    }
}
