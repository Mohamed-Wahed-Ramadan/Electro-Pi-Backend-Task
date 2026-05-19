using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var refreshRepo = _unitOfWork.Repository<Domain.Entities.RefreshToken>();
        var token = await refreshRepo.FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken, cancellationToken);

        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            refreshRepo.Update(token);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success("Logged out successfully.");
    }
}
