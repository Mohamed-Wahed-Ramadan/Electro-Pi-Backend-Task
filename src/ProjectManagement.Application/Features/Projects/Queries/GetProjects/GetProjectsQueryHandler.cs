using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, Result<PaginatedList<ProjectDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetProjectsQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper,
        ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<PaginatedList<ProjectDto>>> Handle(GetProjectsQuery query, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            return Result<PaginatedList<ProjectDto>>.Failure("User not authenticated.");

        var cacheKey = CacheKeys.ProjectsList(
            _currentUser.UserId.Value, query.PageNumber, query.PageSize, query.Search, query.SortBy);

        var cached = await _cache.GetAsync<PaginatedList<ProjectDto>>(cacheKey, cancellationToken);
        if (cached != null)
            return Result<PaginatedList<ProjectDto>>.Success(cached);

        var repo = _unitOfWork.Repository<Project>();
        var projectsQuery = repo.Query()
            .AsNoTracking()
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .Where(p => _currentUser.IsAdmin || p.OwnerUserId == _currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            projectsQuery = projectsQuery.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Description.ToLower().Contains(search));
        }

        projectsQuery = query.SortBy?.ToLower() switch
        {
            "name" => query.SortDescending ? projectsQuery.OrderByDescending(p => p.Name) : projectsQuery.OrderBy(p => p.Name),
            "createdat" => query.SortDescending ? projectsQuery.OrderByDescending(p => p.CreatedAt) : projectsQuery.OrderBy(p => p.CreatedAt),
            _ => projectsQuery.OrderByDescending(p => p.CreatedAt)
        };

        var paginated = await PaginatedList<Project>.CreateAsync(
            projectsQuery, query.PageNumber, query.PageSize, cancellationToken);

        var dtoItems = _mapper.Map<IReadOnlyList<ProjectDto>>(paginated.Items);
        var result = new PaginatedList<ProjectDto>(dtoItems, paginated.TotalCount, paginated.PageNumber, paginated.PageSize);

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return Result<PaginatedList<ProjectDto>>.Success(result);
    }
}
