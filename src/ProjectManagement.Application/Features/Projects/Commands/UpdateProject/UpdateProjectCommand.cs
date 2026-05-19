using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Projects;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(Guid Id, UpdateProjectRequest Request) : IRequest<Result<ProjectDto>>;
