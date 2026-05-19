using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Projects;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    string? SortBy = null,
    bool SortDescending = false) : IRequest<Result<PaginatedList<ProjectDto>>>;
