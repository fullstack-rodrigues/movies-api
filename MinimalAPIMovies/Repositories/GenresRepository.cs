using System.Data;
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
                //select * from Genres order by name 
                var query = "GENRES_GETALL";
                var genres = await connection.QueryAsync<Genre>(query, commandType: System.Data.CommandType.StoredProcedure);
                return genres.ToList();
            }
        }

        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                //SELECT * FROM Genres where Id=@Id
                var query = "GENRES_GETBYID";
                var genre = await connection.QueryFirstOrDefaultAsync<Genre>(query, new {id}, commandType: System.Data.CommandType.StoredProcedure);
                return genre;
            }
        }

        public async Task<List<int>> Exists(List<int> ids)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            foreach (var item in ids)
            {
                dt.Rows.Add(item);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                var genresCreated = await connection.QueryAsync<int>("Genres_GetBySeveralIds", new
                {
                    genresIds = dt
                });
                return genresCreated.ToList();
                
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

        public async Task <bool>Exists(int id, string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>("GetGenres_ByName_Id", new {id, name},
                    commandType: CommandType.StoredProcedure);
                return exists;
            }
        }
    }
}
