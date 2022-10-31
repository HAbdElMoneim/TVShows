
using TVShows.Core.Models;

namespace TVShowsClientsAdapterService.MazeApi.Models
{
    public class PersonResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Birthday { get; set; }

        public Person ToDomain() => new() { Id = Id, Name = Name, Birthday = Birthday };
    }
}