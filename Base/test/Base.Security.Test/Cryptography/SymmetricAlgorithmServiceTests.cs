using Base.Domain.Options.Cryptography;
using Base.Infrastructure.Interface.Security.Cryptography;
using Base.Security.Cryptography;
using Xunit.Abstractions;

namespace Base.Security.Test.Cryptography;

public class SymmetricAlgorithmServiceTests(ITestOutputHelper output)
{
    private static ISymmetricAlgorithmService CreateService()
    {
        SymmetricAlgorithmSetting setting = new();
        return new SymmetricAlgorithmService(
            setting);
    }

    [Fact]
    public void DecryptWithKeyStateUnderTestSuccess()
    {
        // Arrange
        ISymmetricAlgorithmService service = CreateService();
        string encryptedMessage = "d91uABNxMELac/AQyWIAWk1OwwDIKYKOmlcAkYEhJXVg2EMJXfiCztZE2/FlrdTM/AJ+aJ9VP/oFxFo2Q20XdkaTswOM1HvTIyfR7LuRQiHLLL0Pft1kXKlM7eXEKcOtHVBiXs3q5fMwf4CY7/zDqfa3KdL3p32KJIfqpcECBf8oNjJA7Dw20FU1B1DIvriCqt0FcXZVO7/iTU99eA==";

        // Act
        string result = service.DecryptWithKey(
            encryptedMessage);

        // Assert
        output.WriteLine(result);
        Assert.NotNull(result);
    }

    [Fact]
    public void EncryptWithKeyStateUnderTestExpectedBehavior()
    {
        // Arrange
        ISymmetricAlgorithmService service = CreateService();
        string messageToEncrypt = "test";

        // Act
        string result = service.EncryptWithKey(
            messageToEncrypt);

        // Assert
        output.WriteLine(result);
        Assert.NotNull(result);
    }
}
