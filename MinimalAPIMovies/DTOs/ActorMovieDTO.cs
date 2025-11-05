using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.DTOs
{
    public class ActorMovieDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Character { get; set; } = null!;
    }
}
