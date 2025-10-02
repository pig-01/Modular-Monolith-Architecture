using Base.Security.Cryptography;
using Xunit.Abstractions;

namespace Base.Security.Test.Cryptography;

public class CryptographyServiceTests(ITestOutputHelper output)
{

    private static CryptographyService CreateService() => new();

    [Fact]
    public void RC2EncryptStateUnderTestExpectedBehavior()
    {
        // Arrange
        CryptographyService service = CreateService();
        string? unEncryptedString = null;
        string? decryptString = null;

        // Act
        string result = CryptographyService.RC2Encrypt(
            unEncryptedString,
            decryptString);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void RC2EncryptStateUnderTestExpectedBehavior1()
    {
        // Arrange
        CryptographyService service = CreateService();
        string? unEncryptedString = null;
        string? decryptString = null;
        string? salt = null;

        // Act
        string result = CryptographyService.RC2Encrypt(
            unEncryptedString,
            decryptString,
            salt);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void RC2DecryptStateUnderTestExpectedBehavior()
    {
        // Arrange
        CryptographyService service = CreateService();
        string? encryptedString = null;
        string? decryptString = null;

        // Act
        string result = CryptographyService.RC2Decrypt(
            encryptedString,
            decryptString);

        // Assert
        Assert.True(false);
    }

    [Fact]
    public void RC2DecryptStateUnderTestExpectedBehavior1()
    {
        // Arrange
        CryptographyService service = CreateService();
        string? encryptedString = null;
        string? decryptString = null;
        string? salt = null;

        // Act
        string result = CryptographyService.RC2Decrypt(
            encryptedString,
            decryptString,
            salt);

        // Assert
        Assert.True(false);
    }
}
