using FluentValidation;
using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Repositories;

namespace MinimalAPIMovies.Validation
{
    public class CreateGenreDTOValidator: AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenresRepository genresRepository, IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext!.Request.RouteValues["id"];
            var id = 0;
            if (routeId is string routeIdString)
            {
                int.TryParse(routeIdString, out id);
            }
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150)
                .Must(isFirstLetterUpperCase).WithMessage(ValidationUtilities.FirstLetterUpperCase)
                .MustAsync(async(name, _) =>
                {
                    var exists = await genresRepository.Exists(id, name);
                    return !exists;
                }).WithMessage(g => $"a genre with the name {g.Name} already exists");
        }


        public bool isFirstLetterUpperCase(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return true;
            }
            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
