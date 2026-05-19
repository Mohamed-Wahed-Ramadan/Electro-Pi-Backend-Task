using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, Result<PaginatedList<TaskDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetTasksByProjectQueryHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<PaginatedList<TaskDto>>> Handle(GetTasksByProjectQuery query, CancellationToken cancellationToken)
    {
        var projectRepo = _unitOfWork.Repository<Project>();
        var project = await projectRepo.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project == null)
            return Result<PaginatedList<TaskDto>>.Failure("Project not found.");

        if (!_currentUser.IsAdmin && project.OwnerUserId != _currentUser.UserId)
            return Result<PaginatedList<TaskDto>>.Failure("Access denied.");

        var cacheKey = CacheKeys.TasksList(
            query.ProjectId, query.PageNumber, query.PageSize, query.Search, query.Status?.ToString());

        var cached = await _cache.GetAsync<PaginatedList<TaskDto>>(cacheKey, cancellationToken);
        if (cached != null)
            return Result<PaginatedList<TaskDto>>.Success(cached);

        var taskRepo = _unitOfWork.Repository<ProjectTask>();
        var tasksQuery = taskRepo.Query()
            .AsNoTracking()
            .Include(t => t.AssignedUser)
            .Where(t => t.ProjectId == query.ProjectId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            tasksQuery = tasksQuery.Where(t =>
                t.Title.ToLower().Contains(search) ||
                t.Description.ToLower().Contains(search));
        }

        if (query.Status.HasValue)
            tasksQuery = tasksQuery.Where(t => t.Status == query.Status.Value);

        if (query.Priority.HasValue)
            tasksQuery = tasksQuery.Where(t => t.Priority == query.Priority.Value);

        tasksQuery = tasksQuery.OrderByDescending(t => t.CreatedAt);

        var paginated = await PaginatedList<ProjectTask>.CreateAsync(
            tasksQuery, query.PageNumber, query.PageSize, cancellationToken);

        var dtoItems = _mapper.Map<IReadOnlyList<TaskDto>>(paginated.Items);
        var result = new PaginatedList<TaskDto>(dtoItems, paginated.TotalCount, paginated.PageNumber, paginated.PageSize);

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return Result<PaginatedList<TaskDto>>.Success(result);
    }
}
