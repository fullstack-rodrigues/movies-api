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

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = $"delete from Genres where ID=@Id;";
                await connection.ExecuteAsync(query, new {id});
            }
        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exists = await GetById(id);
                if(exists is not null)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "select * from Genres order by name";
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

        public async Task Update(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
 
                var query = @"update Genres set Name=@Name where id=@Id;";
                await connection.ExecuteAsync(query, genre);
            }
        }
    }
}
