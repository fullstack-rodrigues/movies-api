using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor genre);
        Task<List<Actor>> GetAll(string? name);
        Task<Actor?> GetById(int id);
        Task Update(Actor genre);
        Task<bool> Exists(int id);
        Task Delete(int id);
    }
}
