using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Commands.PlanTemplates;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Main.WebApi.Test.Unit.Application.Commands.PlanTemplates;

/// <summary>
/// DeployPlanTemplateCommandHandler 單元測試
/// 測試計畫樣板部署功能的各種情境
/// </summary>
public class DeployPlanTemplateCommandHandlerTests(ITestOutputHelper outputHelper)
{
    private readonly ILogger<DeployPlanTemplateCommandHandler> subLogger = Substitute.For<TestLogger<DeployPlanTemplateCommandHandler>>(outputHelper);

    private readonly IPlanTemplateRepository subPlanTemplateRepository = Substitute.For<IPlanTemplateRepository>();

    private readonly ITimeZoneService subTimeZoneService = Substitute.For<ITimeZoneService>();

    private readonly IUserService<Scuser> subUserService = Substitute.For<IUserService<Scuser>>();

    private DeployPlanTemplateCommandHandler CreateDeployPlanTemplateCommandHandler() => new(
        subLogger,
        subPlanTemplateRepository,
        subTimeZoneService,
        subUserService);

    [Fact(DisplayName = "部署有效版本的計畫樣板時應返回成功")]
    public async Task HandleValidVersionWithTemplatesReturnsTrue()
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        string version = "1.0.0";
        DateTime currentTime = new(2025, 1, 1, 10, 0, 0);
        Scuser currentUser = CreateTestUser();
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationToken cancellationToken = CancellationToken.None;
        int expectedUpdatedCount = 3;

        subUserService.Now(cancellationToken).Returns(currentUser);
        subTimeZoneService.Now.Returns(currentTime);
        subPlanTemplateRepository.DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken)
            .Returns(expectedUpdatedCount);

        // Act
        bool result = await handler.Handle(request, cancellationToken);

        // Assert
        Assert.True(result);
        await subPlanTemplateRepository.Received(1).DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken);
    }

    [Fact(DisplayName = "當版本沒有找到任何樣板時應拋出例外")]
    public async Task HandleVersionWithNoTemplatesThrowsHandleException()
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        string version = "2.0.0";
        DateTime currentTime = new(2025, 1, 1, 10, 0, 0);
        Scuser currentUser = CreateTestUser();
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationToken cancellationToken = CancellationToken.None;

        subUserService.Now(cancellationToken).Returns(currentUser);
        subTimeZoneService.Now.Returns(currentTime);
        subPlanTemplateRepository.DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken)
            .Returns(0);

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal($"版本 {version} 沒有找到任何計畫樣板", exception.Message);
        await subPlanTemplateRepository.Received(1).DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken);
    }

    [Theory(DisplayName = "處理不同版本格式時應呼叫正確的Repository方法")]
    [InlineData("1.0")]
    [InlineData("2.1.5")]
    [InlineData("3.0.0-beta")]
    public async Task HandleDifferentVersionFormatsCallsRepositoryWithCorrectVersion(string version)
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        DateTime currentTime = new(2025, 1, 1, 10, 0, 0);
        Scuser currentUser = CreateTestUser();
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationToken cancellationToken = CancellationToken.None;

        subUserService.Now(cancellationToken).Returns(currentUser);
        subTimeZoneService.Now.Returns(currentTime);
        subPlanTemplateRepository.DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken)
            .Returns(1);

        // Act
        bool result = await handler.Handle(request, cancellationToken);

        // Assert
        Assert.True(result);
        await subPlanTemplateRepository.Received(1).DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken);
    }

    [Fact(DisplayName = "當用戶服務拋出例外時應傳播例外")]
    public async Task HandleUserServiceThrowsExceptionExceptionPropagates()
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        string version = "1.0.0";
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationToken cancellationToken = CancellationToken.None;
        InvalidOperationException expectedException = new("用戶服務異常");

        subUserService.When(x => x.Now(cancellationToken)).Do(x => throw expectedException);

        // Act & Assert
        InvalidOperationException actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(expectedException.Message, actualException.Message);
        await subPlanTemplateRepository.DidNotReceive().DeployPlanTemplatesByVersionAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "當Repository拋出例外時應傳播例外")]
    public async Task HandleRepositoryThrowsExceptionExceptionPropagates()
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        string version = "1.0.0";
        DateTime currentTime = new(2025, 1, 1, 10, 0, 0);
        Scuser currentUser = CreateTestUser();
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationToken cancellationToken = CancellationToken.None;
        InvalidOperationException expectedException = new("資料庫異常");

        subUserService.Now(cancellationToken).Returns(currentUser);
        subTimeZoneService.Now.Returns(currentTime);
        subPlanTemplateRepository.When(x => x.DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken))
            .Do(x => throw expectedException);

        // Act & Assert
        InvalidOperationException actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact(DisplayName = "當取消請求時應拋出HandleException")]
    public async Task HandleCancellationRequestedThrowsHandleException()
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        string version = "1.0.0";
        DateTime currentTime = new(2025, 1, 1, 10, 0, 0);
        Scuser currentUser = CreateTestUser();
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();

        subUserService.Now(cancellationTokenSource.Token).Returns(currentUser);
        subTimeZoneService.Now.Returns(currentTime);

        // Act & Assert
        await Assert.ThrowsAsync<HandleException>(() => handler.Handle(request, cancellationTokenSource.Token));
    }

    [Fact(DisplayName = "處理大量樣板時應正確處理")]
    public async Task HandleLargeNumberOfTemplatesHandlesCorrectly()
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        string version = "1.0.0";
        DateTime currentTime = new(2025, 1, 1, 10, 0, 0);
        Scuser currentUser = CreateTestUser();
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationToken cancellationToken = CancellationToken.None;
        int largeUpdateCount = 10000;

        subUserService.Now(cancellationToken).Returns(currentUser);
        subTimeZoneService.Now.Returns(currentTime);
        subPlanTemplateRepository.DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken)
            .Returns(largeUpdateCount);

        // Act
        bool result = await handler.Handle(request, cancellationToken);

        // Assert
        Assert.True(result);
        await subPlanTemplateRepository.Received(1).DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken);
    }

    [Fact(DisplayName = "有效請求應記錄正確的資訊")]
    public async Task HandleValidRequestLogsCorrectInformation()
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        string version = "1.0.0";
        DateTime currentTime = new(2025, 1, 1, 10, 0, 0);
        Scuser currentUser = CreateTestUser();
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationToken cancellationToken = CancellationToken.None;
        int updatedCount = 5;

        subUserService.Now(cancellationToken).Returns(currentUser);
        subTimeZoneService.Now.Returns(currentTime);
        subPlanTemplateRepository.DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken)
            .Returns(updatedCount);

        // Act
        bool result = await handler.Handle(request, cancellationToken);

        // Assert
        Assert.True(result);

        // Verify logging calls were made (測試日誌記錄是否正確)
        subLogger.Received().LogInformation("開始部署版本 {Version} 的計畫樣板", version);
        subLogger.Received().LogInformation("成功部署版本 {Version} 的 {Count} 個計畫樣板", version, updatedCount);
    }

    [Fact(DisplayName = "沒有找到樣板時應記錄警告")]
    public async Task HandleNoTemplatesFoundLogsWarning()
    {
        // Arrange
        DeployPlanTemplateCommandHandler handler = CreateDeployPlanTemplateCommandHandler();
        string version = "1.0.0";
        DateTime currentTime = new(2025, 1, 1, 10, 0, 0);
        Scuser currentUser = CreateTestUser();
        DeployPlanTemplateCommand request = new() { Version = version };
        CancellationToken cancellationToken = CancellationToken.None;

        subUserService.Now(cancellationToken).Returns(currentUser);
        subTimeZoneService.Now.Returns(currentTime);
        subPlanTemplateRepository.DeployPlanTemplatesByVersionAsync(version, currentTime, currentUser.UserId, cancellationToken)
            .Returns(0);

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(() => handler.Handle(request, cancellationToken));

        // Verify warning was logged
        subLogger.Received().LogWarning("版本 {Version} 沒有找到任何計畫樣板", version);
    }

    /// <summary>
    /// 建立測試用的使用者物件
    /// </summary>
    /// <returns>測試用的 Scuser 物件</returns>
    private static Scuser CreateTestUser() => new()
    {
        UserId = "test-user-001",
        UserName = "測試使用者"
    };
}

/// <summary>
/// DeployPlanTemplateCommand 的基本屬性測試
/// </summary>
public class DeployPlanTemplateCommandTest
{
    [Fact(DisplayName = "DeployPlanTemplateCommand應具有必要屬性")]
    public void DeployPlanTemplateCommandShouldHaveRequiredProperties()
    {
        // Arrange & Act
        DeployPlanTemplateCommand command = new()
        {
            Version = "1.0"
        };

        // Assert
        Assert.Equal("1.0", command.Version);
        Assert.NotNull(command.Version);
    }

    [Fact(DisplayName = "DeployPlanTemplateCommand應具有Required屬性")]
    public void DeployPlanTemplateCommandShouldHaveRequiredAttribute()
    {
        // Arrange
        Type commandType = typeof(DeployPlanTemplateCommand);
        System.Reflection.PropertyInfo? versionProperty = commandType.GetProperty("Version");

        // Assert
        Assert.NotNull(versionProperty);
        object[] requiredAttribute = versionProperty!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false);
        Assert.NotEmpty(requiredAttribute);
    }
}
