using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly string? connectionString;

        public GenresRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = connection.Query("SELECT 1").FirstOrDefault();
            }
            return Task.FromResult(0);
        }
    }
}
