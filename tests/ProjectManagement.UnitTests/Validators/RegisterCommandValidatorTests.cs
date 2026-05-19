using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Features.Auth.Commands.Register;
using Xunit;

namespace ProjectManagement.UnitTests.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new RegisterCommand(new RegisterRequest("invalid", "Password1", "John", "Doe"));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new RegisterCommand(new RegisterRequest("test@example.com", "Password1", "John", "Doe"));
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Too_Short()
    {
        var command = new RegisterCommand(new RegisterRequest("test@example.com", "pass", "John", "Doe"));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }
}
