using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Request.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Request.Description).MaximumLength(5000);
        RuleFor(x => x.Request.ProjectId).NotEmpty();
        RuleFor(x => x.Request.DueDate).GreaterThan(DateTime.UtcNow.AddDays(-1))
            .When(x => x.Request.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}
