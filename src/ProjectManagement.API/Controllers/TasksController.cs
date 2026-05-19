using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.API.Extensions;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;
using ProjectManagement.Application.Features.Tasks.Commands.UpdateTask;
using ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ProjectManagement.Application.Features.Tasks.Commands.UploadTaskAttachment;
using ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.API.Controllers;

[Authorize]
public class TasksController : ApiControllerBase
{
    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Domain.Enums.TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetTasksByProjectQuery(projectId, pageNumber, pageSize, search, status, priority), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CreateTaskCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateTaskCommand(id, request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateTaskStatusCommand(id, request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteTaskCommand(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/attachment")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadAttachment(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var result = await Mediator.Send(
            new UploadTaskAttachmentCommand(id, stream, file.FileName, file.ContentType), cancellationToken);
        return result.ToActionResult();
    }
}
