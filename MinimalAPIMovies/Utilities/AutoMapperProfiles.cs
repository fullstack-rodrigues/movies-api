using AutoMapper;
using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Entities;

namespace MinimalAPIMovies.Utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();
            CreateMap<CreateActorDTO, Actor>().ForMember(p => p.Picture, options => options.Ignore());
            CreateMap<Actor, ActorDTO>();
            CreateMap<Movie, MovieDTO>().ForMember(x=> x.Genres, entity => 
            entity.MapFrom(p => p.Genres.Select(
                gm => new GenreDTO
                {
                    Id = gm.GenreId, 
                    Name = gm.Genre.Name,
                })))
                .ForMember(x=> x.Cast, entity => 
                    entity.MapFrom(p => p.Cast.Select(
                        am => new ActorMovieDTO
                        {
                            Id = am.ActorId,
                            Name = am.Actor.Name,
                            Character = am.Character
                        }))
                );
            CreateMap<CreateMovieDTO, Movie>().ForMember(p => p.Poster, options => options.Ignore()); ;
            CreateMap<CreateCommentDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
            CreateMap<AssignActorMovieDTO, ActorMovie>();
        }
    }
}
