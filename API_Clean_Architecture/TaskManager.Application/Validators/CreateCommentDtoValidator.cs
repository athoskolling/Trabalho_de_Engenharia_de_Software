using FluentValidation;
using TaskManager.Application.Dtos.Comment;
namespace TaskManager.Application.Validators;


    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(500).WithMessage("Content cannot exceed 500 characters.");
        }
    }