using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMovies.Entities;
using MinimalAPIMovies.Repositories;

namespace MinimalAPIMovies.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(120)).Tag("genres-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create);
            group.MapPut("/", Update);
            group.MapDelete("/{id}", Delete);
            return group;
        }

        static async Task<Ok<List<Genre>>> GetAll(IGenresRepository genresRepository)
        {
            var genres = await genresRepository.GetAll();
            return TypedResults.Ok(genres);
        }

        static async Task<Ok<Genre>> GetById(IGenresRepository genresRepository, int id)
        {
            var genre = await genresRepository.GetById(id);
            return TypedResults.Ok(genre);
        }

        static async Task<Created<Genre>> Create(Genre genre, IGenresRepository genresRepository, IOutputCacheStore cacheStore)
        {
            await cacheStore.EvictByTagAsync("genres-get", default);
            await genresRepository.Create(genre);
            return TypedResults.Created($"genres/{genre.Id}", genre);
        }

        static async Task<Results<NotFound, NoContent>> Update(Genre genre, IGenresRepository genresRepository, IOutputCacheStore cacheStore)
        {
            var exist = await genresRepository.Exists(genre.Id);
            if (!exist)
            {
                return TypedResults.NotFound();
            }

            await cacheStore.EvictByTagAsync("genres-get", default);
            await genresRepository.Update(genre);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IGenresRepository genresRepository, IOutputCacheStore cacheStore)
        {
            var exist = await genresRepository.Exists(id);
            if (!exist)
            {
                return TypedResults.NotFound();
            }
            await cacheStore.EvictByTagAsync("genres-get", default);
            await genresRepository.Delete(id);
            return TypedResults.NoContent();
        }
    }
}
