using FluentValidation;
using TodoApp.Application.DTOs;
using TodoApp.Core.Constants;

namespace TodoApp.Application.Validators;

public class SaveTodoItemValidator : AbstractValidator<SaveTodoItemDto>
{
    public SaveTodoItemValidator()
    {
        RuleFor(x => x.title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(x => x.content)
            .NotEmpty()
            .WithMessage("Content is required.");

        RuleFor(x => x.status)
            .Must(BeAValidStatus)
            .When(x => !string.IsNullOrWhiteSpace(x.status))
            .WithMessage("Invalid status value.");

        RuleFor(x => x.priority)
            .Must(BeAValidPriority)
            .When(x => !string.IsNullOrWhiteSpace(x.priority))
            .WithMessage("Invalid priority value.");

        RuleFor(x => x.tag)
            .NotEmpty()
            .WithMessage("Tag is required.");
    }

    private bool BeAValidStatus(string status)
    {
        return new[] { TodoStatus.Pending, TodoStatus.Completed, TodoStatus.Cancelled }
            .Contains(status);
    }

    private bool BeAValidPriority(string priority)
    {
        return new[] { TodoPriority.Low, TodoPriority.Medium, TodoPriority.High }
            .Contains(priority);
    }
}