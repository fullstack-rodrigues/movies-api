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
            CreateMap<Movie, MovieDTO>();
            CreateMap<CreateMovieDTO, Movie>().ForMember(p => p.Poster, options => options.Ignore()); ;
            CreateMap<CreateCommentDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }
    }
}
