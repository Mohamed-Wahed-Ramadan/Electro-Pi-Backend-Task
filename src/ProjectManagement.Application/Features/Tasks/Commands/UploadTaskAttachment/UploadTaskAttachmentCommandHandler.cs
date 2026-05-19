using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Tasks.Commands.UploadTaskAttachment;

public class UploadTaskAttachmentCommandHandler : IRequestHandler<UploadTaskAttachmentCommand, Result<TaskDto>>
{
    private const string Bucket = "task-attachments";
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public UploadTaskAttachmentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage,
        IMapper mapper,
        ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<TaskDto>> Handle(UploadTaskAttachmentCommand command, CancellationToken cancellationToken)
    {
        var taskRepo = _unitOfWork.Repository<ProjectTask>();
        var task = await taskRepo.Query().Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == command.TaskId, cancellationToken);

        if (task == null)
            return Result<TaskDto>.Failure("Task not found.");

        if (!_currentUser.IsAdmin && task.Project.OwnerUserId != _currentUser.UserId)
            return Result<TaskDto>.Failure("Access denied.");

        var objectKey = $"{task.Id}/{Guid.NewGuid()}-{command.FileName}";
        var url = await _fileStorage.UploadAsync(command.FileStream, objectKey, command.ContentType, Bucket, cancellationToken);
        task.AttachmentUrl = url;
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
