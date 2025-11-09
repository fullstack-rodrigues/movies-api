using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);
        Task<List<Genre>> GetAll();
        Task<Genre?> GetById(int id);
        Task Update(Genre genre);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string name);
        Task Delete(int id);

        Task<List<int>> Exists(List<int> ids);
    }
}
