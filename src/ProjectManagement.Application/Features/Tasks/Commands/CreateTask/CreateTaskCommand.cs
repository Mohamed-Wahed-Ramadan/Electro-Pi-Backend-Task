using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Tasks;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(CreateTaskRequest Request) : IRequest<Result<TaskDto>>;
