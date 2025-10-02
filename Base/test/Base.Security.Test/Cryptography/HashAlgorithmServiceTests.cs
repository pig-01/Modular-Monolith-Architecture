using Base.Domain.Options.Cryptography;
using Base.Security.Cryptography;
using Xunit.Abstractions;

namespace Base.Security.Test.Cryptography;

public class HashAlgorithmServiceTests(ITestOutputHelper output)
{
    private static HashAlgorithmService CreateService()
    {
        HashAlgorithmSetting setting = new()
        {
            HashType = "SHA1"
        };
        return new HashAlgorithmService(
            setting);
    }

    [Fact]
    public void HashStateUnderTestExpectedBehavior()
    {
        // Arrange
        HashAlgorithmService service = CreateService();
        string? text = null;

        // Act
        string result = service.Hash(
            text);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void SHA1HashStateUnderTestExpectedBehavior()
    {
        // Arrange
        HashAlgorithmService service = CreateService();
        string? text = null;

        // Act
        string result = service.SHA1Hash(
            text);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void SHA256HashStateUnderTestExpectedBehavior()
    {
        // Arrange
        HashAlgorithmService service = CreateService();
        string? text = null;

        // Act
        string result = service.SHA256Hash(
            text);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void SHA384HashStateUnderTestExpectedBehavior()
    {
        // Arrange
        HashAlgorithmService service = CreateService();
        string? text = null;

        // Act
        string result = service.SHA384Hash(
            text);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void SHA512HashStateUnderTestExpectedBehavior()
    {
        // Arrange
        HashAlgorithmService service = CreateService();
        string? text = null;

        // Act
        string result = service.SHA512Hash(
            text);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void MD5HashStateUnderTestExpectedBehavior()
    {
        // Arrange
        HashAlgorithmService service = CreateService();
        string? text = null;

        // Act
        string result = service.MD5Hash(
            text);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void DisposeStateUnderTestExpectedBehavior()
    {
        // Arrange
        HashAlgorithmService service = CreateService();
        bool disposing = false;

        // Act
        service.Dispose(
            disposing);

        // Assert
        Assert.True(false);
    }
}
