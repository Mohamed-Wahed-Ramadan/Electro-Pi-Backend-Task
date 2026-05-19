using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public CreateProjectCommandHandler(
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

    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            return Result<ProjectDto>.Failure("User not authenticated.");

        var project = new Project
        {
            Name = command.Request.Name,
            Description = command.Request.Description,
            OwnerUserId = _currentUser.UserId.Value
        };

        var repo = _unitOfWork.Repository<Project>();
        await repo.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByPrefixAsync("projects:", cancellationToken);

        var created = await repo.Query()
            .AsNoTracking()
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstAsync(p => p.Id == project.Id, cancellationToken);

        return Result<ProjectDto>.Success(_mapper.Map<ProjectDto>(created));
    }
}
