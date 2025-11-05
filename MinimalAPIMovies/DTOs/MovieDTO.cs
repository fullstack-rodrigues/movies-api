using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool inTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<ActorMovieDTO> Cast { get; set; } = new List<ActorMovieDTO>();
        public List<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
    }
}
