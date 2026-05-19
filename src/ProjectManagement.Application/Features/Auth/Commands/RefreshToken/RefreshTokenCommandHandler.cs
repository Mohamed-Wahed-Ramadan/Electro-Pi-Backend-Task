using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshRepo = _unitOfWork.Repository<Domain.Entities.RefreshToken>();
        var storedToken = await refreshRepo.FirstOrDefaultAsync(
            rt => rt.Token == command.Request.RefreshToken, cancellationToken);

        if (storedToken == null || !storedToken.IsActive)
            return Result<AuthResponse>.Failure("Invalid refresh token.");

        storedToken.RevokedAt = DateTime.UtcNow;
        refreshRepo.Update(storedToken);

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(storedToken.UserId, cancellationToken);
        if (user == null || !user.IsActive)
            return Result<AuthResponse>.Failure("User not found.");
        var tokens = _tokenService.GenerateTokens(user);

        await refreshRepo.AddAsync(new Domain.Entities.RefreshToken
        {
            Token = tokens.RefreshToken,
            ExpiresAt = tokens.RefreshTokenExpiresAt,
            UserId = user.Id,
            ReplacedByToken = tokens.RefreshToken
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString(),
            tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAt));
    }
}
