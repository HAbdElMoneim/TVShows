using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TVShows.Core.Models;
using TVShows.API.EndPoints;
using Xunit;
using TVShows.Application;
using TVShows.Application.ShowsReadData;

namespace TVShows.API.Tests.EndPoints
{
    public class TVShowsControllerTests
    {
        private readonly TVShowsController _tvShowsController;
        private readonly Mock<ITVShowsReadDataService> _showsDataServiceMock;
        private readonly Mock<ILogger<TVShowsController>> _loggerMock;

        public TVShowsControllerTests()
        {
            _loggerMock = new Mock<ILogger<TVShowsController>>();
            _showsDataServiceMock = new Mock<ITVShowsReadDataService>();
            _tvShowsController = new TVShowsController(_showsDataServiceMock.Object, _loggerMock.Object);
        }

        [Theory, AutoData]
        public async Task GivenUseCase_WhenGetShows_FromController_ShowsAreCorrectlyReturned(IReadOnlyCollection<Show> shows)
        {
            // Arrange
            _showsDataServiceMock.Setup(x => x.GetShowsDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(shows);

            // Action
            var result = await _tvShowsController.Get(1,15);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okObjectResult = (OkObjectResult)result;

            okObjectResult.Value.Should().BeEquivalentTo(shows);
        }


        [Theory]
        [InlineAutoData(-1)]
        internal async Task GivenUseCase_WhenGetShows_WithInvalidPageIndex_BadRequestStatusIsReturned(int pageIndexe, List<Show> shows)
        {
            // Arrange
            _showsDataServiceMock.Setup(x => x.GetShowsDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(shows);

            // Action
            var result = await _tvShowsController.Get(pageIndexe, 10);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestObjectResult = (BadRequestObjectResult)result;

            badRequestObjectResult.Value.Should().Be("page size can not be lower than 0");
        }

        [Fact]
        internal async Task GivenUseCase_WhenGetShows_WithUnExpectedError_NotFoundStatusIsReturned()
        {
            // Arrange
            _showsDataServiceMock.Setup(x => x.GetShowsDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Throws(new Exception());
           
            // Action
            var result = await _tvShowsController.Get(1, 10);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
