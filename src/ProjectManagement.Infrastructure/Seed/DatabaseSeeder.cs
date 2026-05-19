using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Persistence.Context;

namespace ProjectManagement.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        for (var attempt = 1; attempt <= 30; attempt++)
        {
            try
            {
                await context.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (attempt < 30)
            {
                logger.LogWarning(ex, "Database not ready, retry {Attempt}/30", attempt);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        if (await context.Users.AnyAsync())
            return;

        logger.LogInformation("Seeding database...");

        var admin = new User
        {
            Email = "admin@projectmanagement.com",
            PasswordHash = passwordHasher.Hash("Admin@12345"),
            FirstName = "System",
            LastName = "Admin",
            Role = UserRole.Admin
        };

        var demoUser = new User
        {
            Email = "user@projectmanagement.com",
            PasswordHash = passwordHasher.Hash("User@12345"),
            FirstName = "Demo",
            LastName = "User",
            Role = UserRole.User
        };

        context.Users.AddRange(admin, demoUser);
        await context.SaveChangesAsync();

        var project = new Project
        {
            Name = "Enterprise Platform",
            Description = "Core platform modernization initiative",
            OwnerUserId = demoUser.Id
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        context.Tasks.AddRange(
            new ProjectTask
            {
                Title = "Design API architecture",
                Description = "Define clean architecture boundaries and contracts",
                Status = Domain.Enums.TaskStatus.Completed,
                Priority = TaskPriority.High,
                ProjectId = project.Id,
                AssignedUserId = demoUser.Id,
                DueDate = DateTime.UtcNow.AddDays(7)
            },
            new ProjectTask
            {
                Title = "Implement authentication",
                Description = "JWT + refresh tokens with role-based access",
                Status = Domain.Enums.TaskStatus.InProgress,
                Priority = TaskPriority.Critical,
                ProjectId = project.Id,
                AssignedUserId = demoUser.Id,
                DueDate = DateTime.UtcNow.AddDays(14)
            },
            new ProjectTask
            {
                Title = "Build dashboard UI",
                Description = "React dashboard with charts and task management",
                Status = Domain.Enums.TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                ProjectId = project.Id,
                DueDate = DateTime.UtcNow.AddDays(21)
            });

        await context.SaveChangesAsync();
        logger.LogInformation("Database seeding completed.");
    }
}
