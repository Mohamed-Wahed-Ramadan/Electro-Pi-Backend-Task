namespace ProjectManagement.Application.DTOs.Dashboard;

public record DashboardStatsDto(
    int TotalProjects,
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    int PendingTasks,
    IReadOnlyList<ProjectTaskSummaryDto> RecentTasks);

public record ProjectTaskSummaryDto(
    Guid Id,
    string Title,
    string ProjectName,
    string Status,
    string Priority,
    DateTime? DueDate);

public record AdminStatsDto(
    int TotalUsers,
    int TotalProjects,
    int TotalTasks,
    int ActiveUsers);
