using FluentValidation;
using MinimalAPIMovies.DTOs;
using MinimalAPIMovies.Repositories;

namespace MinimalAPIMovies.Validation
{
    public class CreateGenreDTOValidator: AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenresRepository genresRepository)
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("The field {PropertyName} is required")
                .MaximumLength(150)
                .Must(isFirstLetterUpperCase).WithMessage("The field {PropertyName} should start uppercase")
                .MustAsync(async(name, _) =>
                {
                    var exists = await genresRepository.Exists(id: 0, name);
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
