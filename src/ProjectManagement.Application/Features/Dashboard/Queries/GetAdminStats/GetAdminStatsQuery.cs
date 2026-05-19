using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Dashboard;

namespace ProjectManagement.Application.Features.Dashboard.Queries.GetAdminStats;

public record GetAdminStatsQuery : IRequest<Result<AdminStatsDto>>;
