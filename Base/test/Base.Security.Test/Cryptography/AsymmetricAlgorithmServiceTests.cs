using Base.Domain.Options.Cryptography;
using Base.Security.Cryptography;
using Xunit.Abstractions;

namespace Base.Security.Test.Cryptography;

public class AsymmetricAlgorithmServiceTests(ITestOutputHelper output)
{
    private static AsymmetricAlgorithmService CreateService()
    {
        AsymmetricAlgorithmSetting asymmetricAlgorithmSetting = new();

        return new AsymmetricAlgorithmService(
            asymmetricAlgorithmSetting);
    }

    [Fact]
    public void ExportPublicKeyStateUnderTestExpectedBehavior()
    {
        // Arrange
        AsymmetricAlgorithmService service = CreateService();

        // Act
        string result = service.ExportPublicKey();

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void ExportPrivateKeyStateUnderTestExpectedBehavior()
    {
        // Arrange
        AsymmetricAlgorithmService service = CreateService();

        // Act
        string result = service.ExportPrivateKey();

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void ImportPublicKeyStateUnderTestExpectedBehavior()
    {
        // Arrange
        AsymmetricAlgorithmService service = CreateService();
        string? pemKey = null;

        // Act
        service.ImportPublicKey(
            pemKey);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void ImportPrivateKeyStateUnderTestExpectedBehavior()
    {
        // Arrange
        AsymmetricAlgorithmService service = CreateService();
        string? pemKey = null;

        // Act
        service.ImportPrivateKey(
            pemKey);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void EncryptStateUnderTestExpectedBehavior()
    {
        // Arrange
        AsymmetricAlgorithmService service = CreateService();
        string? plainText = null;

        // Act
        string result = service.Encrypt(
            plainText);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void DecryptStateUnderTestExpectedBehavior()
    {
        // Arrange
        AsymmetricAlgorithmService service = CreateService();
        string? encryptedText = null;

        // Act
        string result = service.Decrypt(
            encryptedText);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void DisposeStateUnderTestExpectedBehavior()
    {
        // Arrange
        AsymmetricAlgorithmService service = CreateService();
        bool disposing = false;

        // Act
        service.Dispose(
            disposing);

        // Assert
        Assert.True(false);
    }
}
