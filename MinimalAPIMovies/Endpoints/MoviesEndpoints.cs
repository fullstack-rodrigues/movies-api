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
    public static class MoviesEndpoints
    {

        private readonly static string container = "actors";

        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            //group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(120)).Tag("actors-get"));
            //group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery();
            //group.MapPut("/{id:int}", Update).DisableAntiforgery();
            //group.MapDelete("/{id}", Delete);
            return group;
        }

        static async Task<Created<MovieDTO>> Create([FromForm] CreateMovieDTO movieDTO,
    IMoviesRepository moviesRepository, IOutputCacheStore cacheStore,
    IMapper mapper, IFileStorage fileStorage)
        {
            var movie = mapper.Map<Movie>(movieDTO);
            if (movieDTO.Poster is not null)
            {
                var url = await fileStorage.Store(container, movieDTO.Poster);
                movie.Poster = url;
            }
            await cacheStore.EvictByTagAsync("actors-get", default);
            await moviesRepository.Create(movie);
            return TypedResults.Created($"actors/{movie.Id}", mapper.Map<MovieDTO>(movie));
        }
    }
}
