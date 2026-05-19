using ProjectManagement.Infrastructure.Services;
using Xunit;

namespace ProjectManagement.UnitTests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_And_Verify_Should_Work()
    {
        var password = "TestPassword123";
        var hash = _hasher.Hash(password);
        Assert.True(_hasher.Verify(password, hash));
        Assert.False(_hasher.Verify("WrongPassword", hash));
    }
}
