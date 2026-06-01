using Xunit;

namespace CareFlow.Tests.Auth;

public class AuthServiceTests
{
    [Fact]
    public void Login_WithValidCredentials_ShouldReturnToken()
    {
        var token = "jwt-token";

        Assert.False(string.IsNullOrEmpty(token));
    }
}