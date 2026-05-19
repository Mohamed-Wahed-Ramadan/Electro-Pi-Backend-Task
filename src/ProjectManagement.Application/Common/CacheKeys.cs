namespace ProjectManagement.Application.Common;

public static class CacheKeys
{
    public static string ProjectsList(Guid userId, int page, int size, string? search, string? sort) =>
        $"projects:{userId}:{page}:{size}:{search}:{sort}";

    public static string TasksList(Guid projectId, int page, int size, string? search, string? status) =>
        $"tasks:{projectId}:{page}:{size}:{search}:{status}";
}
