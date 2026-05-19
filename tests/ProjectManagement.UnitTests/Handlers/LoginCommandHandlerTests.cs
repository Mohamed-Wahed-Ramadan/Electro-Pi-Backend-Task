using Moq;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Features.Auth.Commands.Login;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;
using Xunit;

namespace ProjectManagement.UnitTests.Handlers;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Failure_For_Invalid_Credentials()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var userRepo = new Mock<IGenericRepository<User>>();
        userRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        unitOfWork.Setup(u => u.Repository<User>()).Returns(userRepo.Object);

        var handler = new LoginCommandHandler(
            unitOfWork.Object,
            new Mock<IPasswordHasher>().Object,
            new Mock<ITokenService>().Object);

        var result = await handler.Handle(new LoginCommand(new LoginRequest("a@b.com", "pass")), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains("Invalid", result.Errors[0]);
    }
}
