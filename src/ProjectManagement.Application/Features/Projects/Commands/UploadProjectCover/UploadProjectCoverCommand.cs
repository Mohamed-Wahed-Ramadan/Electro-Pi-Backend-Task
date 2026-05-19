using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Projects;

namespace ProjectManagement.Application.Features.Projects.Commands.UploadProjectCover;

public record UploadProjectCoverCommand(Guid ProjectId, Stream FileStream, string FileName, string ContentType) : IRequest<Result<ProjectDto>>;
