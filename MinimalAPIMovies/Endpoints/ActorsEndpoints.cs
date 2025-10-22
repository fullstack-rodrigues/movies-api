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
    public static class ActorsEndpoints
    {
        private readonly static string container = "actors";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(120)).Tag("actors-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id}", Delete);
            return group;
        }

        static async Task<Ok<List<ActorDTO>>> GetAll(IActorsRepository actorsRepository, IMapper mapper, [FromQuery] string? name, int page = 1, int itemsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, ItemsPerPage = itemsPerPage };
            var actors = await actorsRepository.GetAll(pagination, name);
            var actorsDTO = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDTO);
        }

        static async Task<Ok<ActorDTO>> GetById(IActorsRepository actorsRepository, int id, IMapper mapper)
        {
            var actor = await actorsRepository.GetById(id);
            return TypedResults.Ok(mapper.Map<ActorDTO>(actor));
        }

        static async Task<Created<ActorDTO>> Create([FromForm] CreateActorDTO actorDTO, 
            IActorsRepository actorsRepository, IOutputCacheStore cacheStore,
            IMapper mapper, IFileStorage fileStorage)
        {
            var actor = mapper.Map<Actor>(actorDTO);
            if(actorDTO.Picture is not null)
            {
                var url = await fileStorage.Store(container, actorDTO.Picture);
                actor.Picture = url;
            }
            await cacheStore.EvictByTagAsync("actors-get", default);
            await actorsRepository.Create(actor);
            return TypedResults.Created($"actors/{actor.Id}", mapper.Map<ActorDTO>(actor));
        }

        static async Task<Results<NotFound, NoContent>> Update([FromForm] CreateActorDTO actorDTO, IActorsRepository actorsRepository, IOutputCacheStore cacheStore, int id, IMapper mapper, IFileStorage fileStorage)
        {
            var actorDb = await actorsRepository.GetById(id);
            if (actorDb is null)
            {
                return TypedResults.NotFound();
            }
            var actor = mapper.Map<Actor>(actorDTO);
            actor.Id = id;
            actor.Picture = actorDb.Picture;
            if(actorDTO.Picture is not null)
            {
                var url = await fileStorage.Edit(actor.Picture, container, actorDTO.Picture);
                actor.Picture = url;
            }

            await cacheStore.EvictByTagAsync("actors-get", default);
            await actorsRepository.Update(actor);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IFileStorage fileStorage, IActorsRepository actorsRepository, IOutputCacheStore cacheStore)
        {
            var exist = await actorsRepository.Exists(id);
            if (!exist)
            {
                return TypedResults.NotFound();
            }
            await cacheStore.EvictByTagAsync("actors-get", default);
            await actorsRepository.Delete(id);
            return TypedResults.NoContent();
        }
    }
}
