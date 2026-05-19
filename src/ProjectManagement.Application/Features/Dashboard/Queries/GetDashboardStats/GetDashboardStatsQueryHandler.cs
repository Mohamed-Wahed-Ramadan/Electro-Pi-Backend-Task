using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Dashboard;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;
using TaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.Application.Features.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            return Result<DashboardStatsDto>.Failure("User not authenticated.");

        var projectRepo = _unitOfWork.Repository<Project>();
        var projectsQuery = projectRepo.Query()
            .Where(p => _currentUser.IsAdmin || p.OwnerUserId == _currentUser.UserId);

        var projectIds = await projectsQuery.Select(p => p.Id).ToListAsync(cancellationToken);
        var totalProjects = projectIds.Count;

        var taskRepo = _unitOfWork.Repository<ProjectTask>();
        var tasksQuery = taskRepo.Query().Where(t => projectIds.Contains(t.ProjectId));

        var totalTasks = await tasksQuery.CountAsync(cancellationToken);
        var completed = await tasksQuery.CountAsync(t => t.Status == TaskStatus.Completed, cancellationToken);
        var inProgress = await tasksQuery.CountAsync(t => t.Status == TaskStatus.InProgress, cancellationToken);
        var pending = await tasksQuery.CountAsync(t => t.Status == TaskStatus.Pending, cancellationToken);

        var recentTasks = await tasksQuery
            .Include(t => t.Project)
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(t => new ProjectTaskSummaryDto(
                t.Id, t.Title, t.Project.Name, t.Status.ToString(), t.Priority.ToString(), t.DueDate))
            .ToListAsync(cancellationToken);

        return Result<DashboardStatsDto>.Success(new DashboardStatsDto(
            totalProjects, totalTasks, completed, inProgress, pending, recentTasks));
    }
}
