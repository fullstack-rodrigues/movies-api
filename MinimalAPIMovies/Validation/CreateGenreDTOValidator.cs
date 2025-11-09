using FluentValidation;
using MinimalAPIMovies.DTOs;

namespace MinimalAPIMovies.Validation
{
    public class CreateGenreDTOValidator: AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("The field {PropertyName} is required")
                .MaximumLength(150)
                .Must(isFirstLetterUpperCase).WithMessage("The field {PropertyName}should start uppercase");
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
