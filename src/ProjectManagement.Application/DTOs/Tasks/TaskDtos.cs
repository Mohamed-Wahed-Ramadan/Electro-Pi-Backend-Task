using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.DTOs.Tasks;

public record TaskDto(
    Guid Id,
    string Title,
    string Description,
    Domain.Enums.TaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid ProjectId,
    Guid? AssignedUserId,
    string? AssignedUserName,
    string? AttachmentUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid ProjectId,
    Guid? AssignedUserId);

public record UpdateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? AssignedUserId);

public record UpdateTaskStatusRequest(Domain.Enums.TaskStatus Status);
