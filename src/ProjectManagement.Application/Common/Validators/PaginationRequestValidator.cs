using FluentValidation;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Common.Validators;

public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
{
    public PaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Search));
    }
}
