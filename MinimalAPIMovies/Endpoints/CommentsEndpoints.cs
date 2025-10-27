using System.ComponentModel;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Entities;
using MinimalAPIMovies.Repositories;
using MinimalAPIMovies.Services;

namespace MinimalAPIMovies.Endpoints
{
    public static class CommentsEndpoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create);
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(120)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById);
            return group;
        }

        static async Task<Created<CommentDTO>> Create(CreateCommentDTO commentDTO,
    ICommentsRepository commentsRepository, IOutputCacheStore cacheStore,
    IMapper mapper)
        {
            var comment = mapper.Map<Comment>(commentDTO);
            await cacheStore.EvictByTagAsync("comments-get", default);
            await commentsRepository.Create(comment);
            return TypedResults.Created($"actors/{comment.Id}", mapper.Map<CommentDTO>(comment));
        }

        static async Task<Ok<List<CommentDTO>>> GetAll(ICommentsRepository commentsRepository, IMapper mapper, [FromQuery] string? body, int page = 1, int itemsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, ItemsPerPage = itemsPerPage };
            var actors = await commentsRepository.GetAll(pagination, body);
            var commentsDTO = mapper.Map<List<CommentDTO>>(actors);
            return TypedResults.Ok(commentsDTO);
        }

        static async Task<Ok<CommentDTO>> GetById(ICommentsRepository commentsRepository, int id, IMapper mapper)
        {
            var actor = await commentsRepository.GetById(id);
            return TypedResults.Ok(mapper.Map<CommentDTO>(actor));
        }
    }
}
