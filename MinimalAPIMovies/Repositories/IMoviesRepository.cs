using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public interface IMoviesRepository
    {
        Task<int> Create(Movie movie);
        Task<List<Movie>> GetAll(PaginationDTO pagination, string? title);
        Task<Movie?> GetById(int id);
        Task Update(Movie movie);
        Task<bool> Exists(int id);
        Task Delete(int id);

        Task Assign(int id, List<int> genresIds);
        Task Assign(int id, List<ActorMovie> actors);
    }
}
