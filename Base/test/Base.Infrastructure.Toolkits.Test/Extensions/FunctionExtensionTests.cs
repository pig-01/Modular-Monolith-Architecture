using Base.Infrastructure.Toolkits.Extensions;
using Xunit.Abstractions;

namespace Base.Infrastructure.Toolkits.Test.Extensions;

public class FunctionExtensionTests(ITestOutputHelper output) : TestBase
{
    private static string func() => throw new Exception("test");

    [Fact]
    public void TryCatchStateUnderTestReturnExceptionMessage()
    {
        // Arrange
        string onException(Exception exception) => exception.Message;

        // Act
        string result = FunctionExtension.TryCatch(
            func,
onException);

        // Assert
        output.WriteLine(result);
        Assert.Equal("test", result);
    }

    [Fact]
    public void TryCatchStateUnderTestReturnCustomerMessage()
    {
        // Arrange

        string onException(Exception exception) => "warm";

        // Act
        string result = FunctionExtension.TryCatch(
            func,
onException);

        // Assert
        output.WriteLine(result);
        Assert.Equal("warm", result);
    }

    [Fact]
    public void TryCatchWithDefaultStateUnderTestReturnCustomerMessage()
    {
        // Arrange
        string defaultValue = "default";

        // Act
        string result = FunctionExtension.TryCatchWithDefault(
            func,
            defaultValue);

        // Assert
        output.WriteLine(result);
        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void TryCatchWithDefaultStateUnderTestReturndefaultMessage()
    {
        // Arrange

        // Act
        string? result = FunctionExtension.TryCatchWithDefault(
            func);

        // Assert
        output.WriteLine(result ?? "");
        Assert.Equal(default, result);
    }
}
