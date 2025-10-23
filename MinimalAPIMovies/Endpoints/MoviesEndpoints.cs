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
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(120)).Tag("movies-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id}", Delete);
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
            await cacheStore.EvictByTagAsync("movies-get", default);
            await moviesRepository.Create(movie);
            return TypedResults.Created($"movies/{movie.Id}", mapper.Map<MovieDTO>(movie));
        }

        static async Task<Ok<List<MovieDTO>>> GetAll(IMoviesRepository moviesRepository, IMapper mapper, [FromQuery] string? title, int page = 1, int itemsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, ItemsPerPage = itemsPerPage };
            var movies = await moviesRepository.GetAll(pagination, title);
            var moviesDTO = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(moviesDTO);
        }
        static async Task<Ok<MovieDTO>> GetById(IMoviesRepository moviesRepository, int id, IMapper mapper)
        {
            var movie = await moviesRepository.GetById(id);
            return TypedResults.Ok(mapper.Map<MovieDTO>(movie));
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IFileStorage fileStorage, IMoviesRepository moviesRepository, IOutputCacheStore cacheStore)
        {
            var exist = await moviesRepository.GetById(id);
            if (exist is null)
            {
                return TypedResults.NotFound();
            }
            await cacheStore.EvictByTagAsync("movies-get", default);
            await fileStorage.Delete(exist.Poster, container);
            await moviesRepository.Delete(id);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Update([FromForm] CreateMovieDTO movieDTO, IMoviesRepository moviesRepository, IOutputCacheStore cacheStore, int id, IMapper mapper, IFileStorage fileStorage)
        {
            var movieDB = await moviesRepository.GetById(id);
            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            var movie = mapper.Map<Movie>(movieDTO);
            movie.Id = id;
            movie.Poster = movieDB.Poster;
            if (movieDTO.Poster is not null)
            {
                var url = await fileStorage.Edit(movie.Poster, container, movieDTO.Poster);
                movie.Poster = url;
            }

            await cacheStore.EvictByTagAsync("movies-get", default);
            await moviesRepository.Update(movie);
            return TypedResults.NoContent();
        }


    }
}
