namespace TVShows.Core.Models
{
    public class Cast
    {
        public Cast()
        {
            Person = new Person();
        }

        public Person Person { get; set; }
    }
}