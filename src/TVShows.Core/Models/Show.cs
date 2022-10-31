namespace TVShows.Core.Models
{
    public class Show
    {
        public Show()
        {
            Cast = new List<Cast>();
            Name = String.Empty;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Cast> Cast { get; set; }
    }
}