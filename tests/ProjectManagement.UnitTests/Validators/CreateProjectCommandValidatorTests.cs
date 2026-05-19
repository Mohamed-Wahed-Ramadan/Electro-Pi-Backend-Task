using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Features.Projects.Commands.CreateProject;
using Xunit;

namespace ProjectManagement.UnitTests.Validators;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        var command = new CreateProjectCommand(new CreateProjectRequest("", "Description"));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = new CreateProjectCommand(new CreateProjectRequest("My Project", "A great project"));
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }
}
