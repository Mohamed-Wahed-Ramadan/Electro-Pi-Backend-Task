using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand(Guid Id, UpdateTaskRequest Request) : IRequest<Result<TaskDto>>;
