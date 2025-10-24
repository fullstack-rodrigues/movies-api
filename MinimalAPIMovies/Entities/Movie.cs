namespace MinimalAPIMovies.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool inTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster {  get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
