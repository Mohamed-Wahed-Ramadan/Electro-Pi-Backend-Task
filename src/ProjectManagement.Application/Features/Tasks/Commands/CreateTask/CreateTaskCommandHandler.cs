using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public CreateTaskCommandHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<TaskDto>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var projectRepo = _unitOfWork.Repository<Project>();
        var project = await projectRepo.GetByIdAsync(command.Request.ProjectId, cancellationToken);
        if (project == null)
            return Result<TaskDto>.Failure("Project not found.");

        if (!_currentUser.IsAdmin && project.OwnerUserId != _currentUser.UserId)
            return Result<TaskDto>.Failure("Access denied.");

        var task = new ProjectTask
        {
            Title = command.Request.Title,
            Description = command.Request.Description,
            Priority = command.Request.Priority,
            DueDate = command.Request.DueDate,
            ProjectId = command.Request.ProjectId,
            AssignedUserId = command.Request.AssignedUserId
        };

        var taskRepo = _unitOfWork.Repository<ProjectTask>();
        await taskRepo.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByPrefixAsync($"tasks:{command.Request.ProjectId}", cancellationToken);
        await _cache.RemoveByPrefixAsync("projects:", cancellationToken);

        var created = await taskRepo.Query()
            .AsNoTracking()
            .Include(t => t.AssignedUser)
            .FirstAsync(t => t.Id == task.Id, cancellationToken);

        return Result<TaskDto>.Success(_mapper.Map<TaskDto>(created));
    }
}
