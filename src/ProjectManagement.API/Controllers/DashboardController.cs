using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.API.Extensions;
using ProjectManagement.Application.Features.Dashboard.Queries.GetAdminStats;
using ProjectManagement.Application.Features.Dashboard.Queries.GetDashboardStats;

namespace ProjectManagement.API.Controllers;

[Authorize]
public class DashboardController : ApiControllerBase
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAdminStats(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAdminStatsQuery(), cancellationToken);
        return result.ToActionResult();
    }
}
