using Microsoft.AspNetCore.Mvc;
using System.Net;
using TVShows.Application;
using TVShows.Application.ShowsReadData;

namespace TVShows.API.EndPoints
{
    [ApiController]
    [Route("[controller]")]
    public class TVShowsController : ControllerBase
    {
        private readonly ITVShowsReadDataService _showsDataService;
        private readonly ILogger<TVShowsController> _logger;

        public TVShowsController(ITVShowsReadDataService showsDataService, ILogger<TVShowsController> logger)
        {
            _showsDataService = showsDataService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to get shows with the cast information
        /// </summary>
        /// <param name="pageSize">Number of records required in one single page</param>
        /// <param name="pageIndex">Index of the page</param>
        /// <param name="cancellationToken">cancellation Token</param>
        /// <returns>List of shows including list of casts for each show</returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return BadRequest("page size can not be lower than 0");
                }

                var result = await _showsDataService.GetShowsDataAsync(pageIndex, pageSize, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error {error}", ex);
                return NotFound();
            }
        }
    }
}