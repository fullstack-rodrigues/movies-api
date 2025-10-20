namespace MinimalAPIMovies.DTOs
{
    public class CreateActorDTO
    {
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        public string? Picture { get; set; }
    }
}
