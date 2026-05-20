namespace ProjectManagement.Application.DTOs.Projects;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid OwnerUserId { get; set; }
    public string OwnerName { get; set; } = default!;
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TaskCount { get; set; }
}

public record CreateProjectRequest(string Name, string Description);
public record UpdateProjectRequest(string Name, string Description);
