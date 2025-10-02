using Base.Infrastructure.Toolkits.Extensions;
using Xunit.Abstractions;

namespace Base.Infrastructure.Toolkits.Test.Extensions;

public class EnvironmentExtensionTests(ITestOutputHelper outputHelper)
{

    [Fact]
    public void IsDevelopmentStateUnderTestCheckEnvironment()
    {
        // Arrange
        string variable = "ASPNETCORE_ENVIRONMENT";

        // Act
        bool result = EnvironmentExtension.IsDevelopment(
            variable);

        outputHelper.WriteLine(result.ToString());
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDevelopmentStateUnderTestErrorVariable()
    {
        // Arrange
        string variable = "ASPNETCORE_ENV";

        // Act
        bool result = EnvironmentExtension.IsDevelopment(
            variable);

        outputHelper.WriteLine(result.ToString());
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsStagingStateUnderTestExpectedBehavior()
    {
        // Arrange
        string variable = "ASPNETCORE_ENVIRONMENT";

        // Act
        bool result = EnvironmentExtension.IsStaging(
            variable);

        outputHelper.WriteLine(result.ToString());
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsStagingStateUnderTestErrorVariable()
    {
        // Arrange
        string variable = "ASPNORE_ENV";

        // Act
        bool result = EnvironmentExtension.IsStaging(
            variable);

        outputHelper.WriteLine(result.ToString());
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsProductionStateUnderTestExpectedBehavior()
    {
        // Arrange
        string variable = "ASPNETCORE_ENVIRONMENT";

        // Act
        bool result = EnvironmentExtension.IsProduction(
            variable);

        outputHelper.WriteLine(result.ToString());
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsProductionStateUnderTestErrorVariable()
    {
        // Arrange
        string variable = "ASPNETCONV";

        // Act
        bool result = EnvironmentExtension.IsProduction(
            variable);

        outputHelper.WriteLine(result.ToString());
        // Assert
        Assert.False(result);
    }
}
