using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public class ActorsRepository: IActorsRepository
    {
        private readonly string? connectionString;

        public ActorsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> Create(Actor actor)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"INSERT INTO Actors (Name, DateOfBirth, Picture ) VALUES (@Name, @DateOfBirth, @Picture); SELECT SCOPE_IDENTITY()";

                var id = await connection.QuerySingleAsync<int>(query, new
                {
                    actor.Name,
                    actor.DateOfBirth,
                    actor.Picture
                });
                actor.Id = id;
                return id;
            }
        }

        public async Task<List<Actor>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                
                var query = "select * from actors";
                var actors = await connection.QueryAsync<Actor>(query);
                return actors.ToList();
            }
        }

        public async Task<Actor?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"SELECT * FROM actors where Id=@Id";
                var actor = await connection.QueryFirstOrDefaultAsync<Actor>(query, new { id });
                return actor;
            }
        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exists = await GetById(id);
                if (exists is not null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = $"delete from Actors where ID=@Id;";
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task Update(Actor actor)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var query = @"update Actors set Name=@Name, Picture=@Picture,DateOfBirth=@DateOfBirth  where id=@Id;";
                await connection.ExecuteAsync(query, new { actor.Name, actor.DateOfBirth, actor.Picture, actor.Id });
            }
        }

    }
}
