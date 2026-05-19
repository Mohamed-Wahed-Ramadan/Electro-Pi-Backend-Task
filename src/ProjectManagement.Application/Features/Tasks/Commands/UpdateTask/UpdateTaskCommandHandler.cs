using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public UpdateTaskCommandHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<TaskDto>> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<ProjectTask>();
        var task = await taskRepo.Query().Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);

        if (task == null)
            return Result<TaskDto>.Failure("Task not found.");

        if (!_currentUser.IsAdmin && task.Project.OwnerUserId != _currentUser.UserId)
            return Result<TaskDto>.Failure("Access denied.");

        task.Title = command.Request.Title;
        task.Description = command.Request.Description;
        task.Priority = command.Request.Priority;
        task.DueDate = command.Request.DueDate;
        task.AssignedUserId = command.Request.AssignedUserId;
        task.UpdatedAt = DateTime.UtcNow;
        taskRepo.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByPrefixAsync($"tasks:{task.ProjectId}", cancellationToken);

        var updated = await taskRepo.Query()
            .AsNoTracking()
            .Include(t => t.AssignedUser)
            .FirstAsync(t => t.Id == task.Id, cancellationToken);

        return Result<TaskDto>.Success(_mapper.Map<TaskDto>(updated));
    }
}
