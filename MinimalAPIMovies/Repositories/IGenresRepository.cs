using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);
    }
}
