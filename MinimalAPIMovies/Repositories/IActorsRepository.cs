using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor genre);
        Task<List<Actor>> GetAll(PaginationDTO pagination, string? name);
        Task<Actor?> GetById(int id);
        Task Update(Actor genre);
        Task<bool> Exists(int id);
        Task Delete(int id);
    }
}
