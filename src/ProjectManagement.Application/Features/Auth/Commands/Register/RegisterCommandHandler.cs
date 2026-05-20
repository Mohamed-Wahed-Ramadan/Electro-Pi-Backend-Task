using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<User>();
        var existing = await repo.FirstOrDefaultAsync(u => u.Email == command.Request.Email.ToLowerInvariant(), cancellationToken);
        if (existing != null)
            return Result<AuthResponse>.Failure("Email is already registered.");

        var user = new User
        {
            Email = command.Request.Email.ToLowerInvariant(),
            PasswordHash = _passwordHasher.Hash(command.Request.Password),
            FirstName = command.Request.FirstName,
            LastName = command.Request.LastName,
            Role = UserRole.User
        };

        await repo.AddAsync(user, cancellationToken);

        var tokens = _tokenService.GenerateTokens(user);
        var refreshRepo = _unitOfWork.Repository<ProjectManagement.Domain.Entities.RefreshToken>();
        await refreshRepo.AddAsync(new ProjectManagement.Domain.Entities.RefreshToken
        {
            Token = tokens.RefreshToken,
            ExpiresAt = tokens.RefreshTokenExpiresAt,
            UserId = user.Id
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AuthResponse>.Success(MapResponse(user, tokens));
    }

    private static AuthResponse MapResponse(User user, TokenResponse tokens) =>
        new(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString(),
            tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAt);
}
