using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<User>();
        var user = await repo.FirstOrDefaultAsync(
            u => u.Email == command.Request.Email.ToLowerInvariant(), cancellationToken);

        if (user == null || !user.IsActive || !_passwordHasher.Verify(command.Request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Invalid email or password.");

        var tokens = _tokenService.GenerateTokens(user);
        var refreshRepo = _unitOfWork.Repository<ProjectManagement.Domain.Entities.RefreshToken>();
        await refreshRepo.AddAsync(new ProjectManagement.Domain.Entities.RefreshToken
        {
            Token = tokens.RefreshToken,
            ExpiresAt = tokens.RefreshTokenExpiresAt,
            UserId = user.Id
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString(),
            tokens.AccessToken, tokens.RefreshToken, tokens.AccessTokenExpiresAt));
    }
}
