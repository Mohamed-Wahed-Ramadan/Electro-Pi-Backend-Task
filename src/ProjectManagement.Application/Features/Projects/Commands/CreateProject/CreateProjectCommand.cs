using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Projects;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(CreateProjectRequest Request) : IRequest<Result<ProjectDto>>;
