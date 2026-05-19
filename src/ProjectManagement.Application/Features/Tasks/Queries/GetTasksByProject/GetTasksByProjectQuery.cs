using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public record GetTasksByProjectQuery(
    Guid ProjectId,
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    Domain.Enums.TaskStatus? Status = null,
    TaskPriority? Priority = null) : IRequest<Result<PaginatedList<TaskDto>>>;
