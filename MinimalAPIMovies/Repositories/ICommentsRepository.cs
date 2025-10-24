using System.Xml.Linq;
using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public interface ICommentsRepository
    {
        Task<int> Create(Comment comment);
        Task<List<Comment>> GetAll(PaginationDTO pagination, string? search);
        Task<Comment?> GetById(int id);
        Task Update(Comment comment);
        Task<bool> Exists(int id);
        Task Delete(int id);
    }
}
