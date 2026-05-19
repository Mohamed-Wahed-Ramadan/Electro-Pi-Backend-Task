using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;

namespace ProjectManagement.Application.Features.Tasks.Commands.UploadTaskAttachment;

public record UploadTaskAttachmentCommand(Guid TaskId, Stream FileStream, string FileName, string ContentType) : IRequest<Result<TaskDto>>;
