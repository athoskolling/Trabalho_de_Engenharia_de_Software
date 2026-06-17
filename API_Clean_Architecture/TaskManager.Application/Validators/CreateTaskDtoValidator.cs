using FluentValidation;
using TaskManager.Application.Dtos.Task;
using TaskManager.Domain.Enums;
using System;

namespace TaskManager.Application.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be greater than the current date.")
            .When(x => x.DueDate != null);

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value. Allowed values are: Low, Medium, High.")
            .When(x => x.Priority != null);

        RuleFor(x => x.AssignedToId)
            .NotEqual(Guid.Empty).WithMessage("Assigned user ID cannot be empty.")
            .When(x => x.AssignedToId != null);
    }
}
