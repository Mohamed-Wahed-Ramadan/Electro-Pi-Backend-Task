using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            return Result.Failure("User not authenticated.");

        var repo = _unitOfWork.Repository<User>();
        var user = await repo.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user == null)
            return Result.Failure("User not found.");

        if (!_passwordHasher.Verify(command.Request.CurrentPassword, user.PasswordHash))
            return Result.Failure("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.Hash(command.Request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        repo.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Password changed successfully.");
    }
}
