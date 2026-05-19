using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public UpdateProjectCommandHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Project>();
        var project = await repo.GetByIdAsync(command.Id, cancellationToken);
        if (project == null)
            return Result<ProjectDto>.Failure("Project not found.");

        if (!_currentUser.IsAdmin && project.OwnerUserId != _currentUser.UserId)
            return Result<ProjectDto>.Failure("Access denied.");

        project.Name = command.Request.Name;
        project.Description = command.Request.Description;
        project.UpdatedAt = DateTime.UtcNow;
        repo.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByPrefixAsync("projects:", cancellationToken);

        var updated = await repo.Query()
            .AsNoTracking()
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstAsync(p => p.Id == project.Id, cancellationToken);

        return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(updated));
    }
}
