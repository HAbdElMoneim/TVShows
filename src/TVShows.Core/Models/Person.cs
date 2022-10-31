namespace TVShows.Core.Models
{
    public class Person
    {
        public Person()
        {
            Name = String.Empty;
            Birthday = String.Empty;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Birthday { get; set; }
    }
}