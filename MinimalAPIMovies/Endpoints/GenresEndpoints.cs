using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMovies.DTOs;
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
            group.MapPut("/{id:int}", Update);
            group.MapDelete("/{id}", Delete);
            return group;
        }

        static async Task<Ok<List<GenreDTO>>> GetAll(IGenresRepository genresRepository, IMapper mapper)
        {
            var genres = await genresRepository.GetAll();
            var genresDTO = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genresDTO);
        }

        static async Task<Ok<GenreDTO>> GetById(IGenresRepository genresRepository, int id, IMapper mapper)
        {
            var genre = await genresRepository.GetById(id);
            return TypedResults.Ok(mapper.Map<GenreDTO>(genre));
        }

        static async Task<Created<GenreDTO>> Create(CreateGenreDTO genreDTO, IGenresRepository genresRepository, IOutputCacheStore cacheStore, IMapper mapper)
        {
            var genre = mapper.Map<Genre>(genreDTO);
            await cacheStore.EvictByTagAsync("genres-get", default);
            await genresRepository.Create(genre);
            return TypedResults.Created($"genres/{genre.Id}", mapper.Map<GenreDTO>(genre));
        }

        static async Task<Results<NotFound, NoContent>> Update(CreateGenreDTO genreDTO, IGenresRepository genresRepository, IOutputCacheStore cacheStore, int id, IMapper mapper)
        {
            var genre = mapper.Map<Genre>(genreDTO);
            genre.Id = id;
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
