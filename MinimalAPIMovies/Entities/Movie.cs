namespace MinimalAPIMovies.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool inTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster {  get; set; }
    }
}
