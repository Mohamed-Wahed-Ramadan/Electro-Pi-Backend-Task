using FluentValidation;

namespace ProjectManagement.Application.Features.Projects.Commands.UploadProjectCover;

public class UploadProjectCoverCommandValidator : AbstractValidator<UploadProjectCoverCommand>
{
    private static readonly string[] AllowedTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];
    private const long MaxSize = 5 * 1024 * 1024;

    public UploadProjectCoverCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.ContentType).Must(t => AllowedTypes.Contains(t.ToLower()))
            .WithMessage("Only JPEG, PNG, WebP, and GIF images are allowed.");
        RuleFor(x => x.FileStream).Must(s => s.Length <= MaxSize)
            .WithMessage("File size must not exceed 5MB.");
    }
}
