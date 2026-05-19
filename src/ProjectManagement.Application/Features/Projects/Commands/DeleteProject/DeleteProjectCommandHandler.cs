using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<Result> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Project>();
        var project = await repo.Query()
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (project == null)
            return Result.Failure("Project not found.");

        if (!_currentUser.IsAdmin && project.OwnerUserId != _currentUser.UserId)
            return Result.Failure("Access denied.");

        var taskRepo = _unitOfWork.Repository<ProjectTask>();
        foreach (var task in project.Tasks)
            taskRepo.Remove(task);

        repo.Remove(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByPrefixAsync("projects:", cancellationToken);
        await _cache.RemoveByPrefixAsync($"tasks:{command.Id}", cancellationToken);

        return Result.Success("Project deleted successfully.");
    }
}
