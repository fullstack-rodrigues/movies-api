using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public class MoviesRepository: IMoviesRepository
    {
        private readonly string? connectionString;
        private readonly HttpContext httpContext;

        public MoviesRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<int> Create(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"INSERT INTO Movies (Title, InTheaters, ReleaseDate, Poster) 
                    VALUES (@Title, @InTheaters, @ReleaseDate, @Poster); SELECT SCOPE_IDENTITY()";

                var id = await connection.QuerySingleAsync<int>(query, new
                {
                    movie.Title,
                    movie.inTheaters,
                    movie.ReleaseDate,
                    movie.Poster
                });
                movie.Id = id;
                return id;
            }
        }

        public async Task<List<Movie>> GetAll(PaginationDTO pagination, string? title)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var query = "select * from Movies";
                var countQuery = "select count(*) from Movies";
                if (title is not null)
                {
                    query += @" where Title like @title";
                    countQuery += @" where Title like @title";
                }
                query += " ORDER BY Id OFFSET (@page - 1) * @itemsPerPage ROWS FETCH NEXT @itemsPerPage ROWS ONLY";
                var parameters = new { title = $"%{title}%", page = pagination.Page, itemsPerPage = pagination.ItemsPerPage };

                var movies = await connection.QueryAsync<Movie>(query, parameters);
                var moviesCount = await connection.QuerySingleAsync<int>(countQuery, parameters);
                var totalOfPages = Math.Ceiling((double)moviesCount / pagination.ItemsPerPage);
                httpContext.Response.Headers.Append("totalOfRecords", moviesCount.ToString());
                httpContext.Response.Headers.Append("totalOfPages", totalOfPages.ToString());
                return movies.ToList();
            }
        }



        public async Task<Movie?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"SELECT * FROM Movies where Id=@Id; SELECT * FROM Comments where movieID=@Id";
                using (var multi = await connection.QueryMultipleAsync(query, new { id }))
                {
                var movie = await multi.ReadFirstAsync<Movie>();
                var comments = await multi.ReadAsync<Comment>();
                    movie.Comments = comments.ToList();

                return movie;
                }
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
                var query = $"delete from movies where ID=@Id;";
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task Update(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var query = @"update Movies set Title=@Title, InTheaters=@InTheaters,ReleaseDate=@ReleaseDate, Poster=@Poster  where id=@Id;";

                await connection.ExecuteAsync(query, new { movie.Title, movie.inTheaters, movie.ReleaseDate, movie.Poster, movie.Id });
            }
        }
    }
}
