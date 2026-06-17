using FluentValidation;
using TaskManager.Application.Dtos.Task;
using TaskManager.Domain.Enums;
using System;

namespace TaskManager.Application.Validators;

public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.")
            .When(x => x.Title != null);

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

        RuleFor(x => x.State)
            .IsInEnum().WithMessage("Invalid status value. Allowed values are: Pending, InProgress, InReview, Completed.")
            .When(x => x.State != null);
    }
}

