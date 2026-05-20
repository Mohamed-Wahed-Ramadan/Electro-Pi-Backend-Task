using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Users", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Projects",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                OwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CoverImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Projects", x => x.Id);
                table.ForeignKey(
                    name: "FK_Projects_Users_OwnerUserId",
                    column: x => x.OwnerUserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "RefreshTokens",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_RefreshTokens_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Tasks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AssignedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                AttachmentUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tasks", x => x.Id);
                table.ForeignKey(
                    name: "FK_Tasks_Projects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "Projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Tasks_Users_AssignedUserId",
                    column: x => x.AssignedUserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex(name: "IX_Users_Email", table: "Users", column: "Email", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Projects_CreatedAt", table: "Projects", column: "CreatedAt");
        migrationBuilder.CreateIndex(name: "IX_Projects_Name", table: "Projects", column: "Name");
        migrationBuilder.CreateIndex(name: "IX_Projects_OwnerUserId", table: "Projects", column: "OwnerUserId");
        migrationBuilder.CreateIndex(name: "IX_RefreshTokens_Token", table: "RefreshTokens", column: "Token", unique: true);
        migrationBuilder.CreateIndex(name: "IX_RefreshTokens_UserId", table: "RefreshTokens", column: "UserId");
        migrationBuilder.CreateIndex(name: "IX_Tasks_AssignedUserId", table: "Tasks", column: "AssignedUserId");
        migrationBuilder.CreateIndex(name: "IX_Tasks_Priority", table: "Tasks", column: "Priority");
        migrationBuilder.CreateIndex(name: "IX_Tasks_ProjectId", table: "Tasks", column: "ProjectId");
        migrationBuilder.CreateIndex(name: "IX_Tasks_Status", table: "Tasks", column: "Status");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "RefreshTokens");
        migrationBuilder.DropTable(name: "Tasks");
        migrationBuilder.DropTable(name: "Projects");
        migrationBuilder.DropTable(name: "Users");
    }
}
