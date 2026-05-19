using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Dashboard;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Dashboard.Queries.GetAdminStats;

public class GetAdminStatsQueryHandler : IRequestHandler<GetAdminStatsQuery, Result<AdminStatsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminStatsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<AdminStatsDto>> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var projectRepo = _unitOfWork.Repository<Project>();
        var taskRepo = _unitOfWork.Repository<ProjectTask>();

        var totalUsers = await userRepo.Query().CountAsync(cancellationToken);
        var activeUsers = await userRepo.Query().CountAsync(u => u.IsActive, cancellationToken);
        var totalProjects = await projectRepo.Query().CountAsync(cancellationToken);
        var totalTasks = await taskRepo.Query().CountAsync(cancellationToken);

        return Result<AdminStatsDto>.Success(new AdminStatsDto(
            totalUsers, totalProjects, totalTasks, activeUsers));
    }
}
