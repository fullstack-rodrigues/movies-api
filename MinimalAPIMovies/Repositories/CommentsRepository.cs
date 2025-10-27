using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {

        private readonly string? connectionString;
        private readonly HttpContext httpContext;

        public CommentsRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            httpContext = httpContextAccessor.HttpContext!;
        }
        public async  Task<int> Create(Comment comment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"INSERT INTO Comments (Body, MovieId ) VALUES (@Body, @MovieId); SELECT SCOPE_IDENTITY()";

                var id = await connection.QuerySingleAsync<int>(query, new
                {
                    comment.Body,
                    comment.MovieId,
                });
                comment.Id = id;
                return id;
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = $"delete from Comments where ID=@Id;";
                await connection.ExecuteAsync(query, new { id });
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

        public async Task<List<Comment>> GetAll(PaginationDTO pagination, string? search)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var query = "select * from Comments";
                var countQuery = "select count(*) from Comments";
                if (search is not null)
                {
                    query += @" where body like @search";
                    countQuery += @" where body like @search";
                }
                query += " ORDER BY Id OFFSET (@page - 1) * @itemsPerPage ROWS FETCH NEXT @itemsPerPage ROWS ONLY";
                var parameters = new { search = $"%{search}%", page = pagination.Page, itemsPerPage = pagination.ItemsPerPage };

                var comments = await connection.QueryAsync<Comment>(query, parameters);
                var commentsCount = await connection.QuerySingleAsync<int>(countQuery, parameters);
                var totalOfPages = Math.Ceiling((double)commentsCount / pagination.ItemsPerPage);
                httpContext.Response.Headers.Append("totalOfRecords", commentsCount.ToString());
                httpContext.Response.Headers.Append("totalOfPages", totalOfPages.ToString());
                return comments.ToList();
            }
        }

        public async Task<Comment?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"SELECT * FROM Comments where Id=@Id";
                var comment = await connection.QueryFirstOrDefaultAsync<Comment>(query, new { id });
                return comment;
            }
        }

        public async Task Update(Comment comment)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var query = @"update Comments set Body=@Body, MovieId=@MovieId where id=@Id;";
                await connection.ExecuteAsync(query, new { comment.Body, comment.MovieId, comment.Id });
            }
        }
    }
}
