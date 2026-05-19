using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.API.Extensions;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Features.Auth.Commands.ChangePassword;
using ProjectManagement.Application.Features.Auth.Commands.Login;
using ProjectManagement.Application.Features.Auth.Commands.Logout;
using ProjectManagement.Application.Features.Auth.Commands.RefreshToken;
using ProjectManagement.Application.Features.Auth.Commands.Register;

namespace ProjectManagement.API.Controllers;

public class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new RegisterCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new LoginCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new RefreshTokenCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new ChangePasswordCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
