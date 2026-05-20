using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.DTOs.Tasks;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Domain.Enums.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? AssignedUserName { get; set; }
    public string? AttachmentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

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
