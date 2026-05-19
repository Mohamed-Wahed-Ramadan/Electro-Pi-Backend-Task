using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Commands.UploadTaskAttachment;

public class UploadTaskAttachmentCommandValidator : AbstractValidator<UploadTaskAttachmentCommand>
{
    private static readonly string[] AllowedTypes =
    [
        "image/jpeg", "image/png", "image/webp", "application/pdf",
        "text/plain", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    ];
    private const long MaxSize = 10 * 1024 * 1024;

    public UploadTaskAttachmentCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.ContentType).Must(t => AllowedTypes.Contains(t.ToLower()))
            .WithMessage("File type not allowed.");
        RuleFor(x => x.FileStream).Must(s => s.Length <= MaxSize)
            .WithMessage("File size must not exceed 10MB.");
    }
}
