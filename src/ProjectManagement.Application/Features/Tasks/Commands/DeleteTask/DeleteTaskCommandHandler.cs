using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<Result> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<ProjectTask>();
        var task = await taskRepo.Query().Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

        if (task == null)
            return Result.Failure("Task not found.");

        if (!_currentUser.IsAdmin && task.Project.OwnerUserId != _currentUser.UserId)
            return Result.Failure("Access denied.");

        taskRepo.Remove(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByPrefixAsync($"tasks:{task.ProjectId}", cancellationToken);

        return Result.Success("Task deleted successfully.");
    }
}
