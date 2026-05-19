using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(Guid Id, UpdateTaskStatusRequest Request) : IRequest<Result<TaskDto>>;
