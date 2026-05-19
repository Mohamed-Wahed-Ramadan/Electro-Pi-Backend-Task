using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Projects.Commands.UploadProjectCover;

public class UploadProjectCoverCommandHandler : IRequestHandler<UploadProjectCoverCommand, Result<ProjectDto>>
{
    private const string Bucket = "project-covers";
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public UploadProjectCoverCommandHandler(
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

    public async Task<Result<ProjectDto>> Handle(UploadProjectCoverCommand command, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Project>();
        var project = await repo.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null)
            return Result<ProjectDto>.Failure("Project not found.");

        if (!_currentUser.IsAdmin && project.OwnerUserId != _currentUser.UserId)
            return Result<ProjectDto>.Failure("Access denied.");

        var objectKey = $"{project.Id}/{Guid.NewGuid()}-{command.FileName}";
        var url = await _fileStorage.UploadAsync(command.FileStream, objectKey, command.ContentType, Bucket, cancellationToken);
        project.CoverImageUrl = url;
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
