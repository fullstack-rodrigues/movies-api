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

        public async Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"INSERT INTO Genres (Name) VALUES (@Name); SELECT SCOPE_IDENTITY()";

                var id = await connection.QuerySingleAsync<int>(query, genre);
                genre.Id = id;
                return id;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "select * from Genres";
                var genres = await connection.QueryAsync<Genre>(query);
                return genres.ToList();
            }
        }

        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"SELECT * FROM Genres where Id=@Id";
                var genre = await connection.QueryFirstOrDefaultAsync<Genre>(query, new {id});
                return genre;
            }
        }
    }
}
