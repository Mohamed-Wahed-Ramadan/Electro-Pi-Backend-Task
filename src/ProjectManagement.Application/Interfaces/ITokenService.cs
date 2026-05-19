using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Interfaces;

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}

public interface ITokenService
{
    TokenResponse GenerateTokens(User user);
    Guid? GetUserIdFromExpiredToken(string token);
}
