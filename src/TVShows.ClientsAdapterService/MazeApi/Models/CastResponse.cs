
using TVShows.Core.Models;

namespace TVShowsClientsAdapterService.MazeApi.Models
{
    public class CastResponse
    {
        public PersonResponse Person { get; set; }

        public Cast ToDomain() => new() { Person = Person.ToDomain() };
    }
}