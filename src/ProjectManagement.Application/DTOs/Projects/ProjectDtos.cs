namespace ProjectManagement.Application.DTOs.Projects;

public record ProjectDto(
    Guid Id,
    string Name,
    string Description,
    Guid OwnerUserId,
    string OwnerName,
    string? CoverImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int TaskCount);

public record CreateProjectRequest(string Name, string Description);
public record UpdateProjectRequest(string Name, string Description);
