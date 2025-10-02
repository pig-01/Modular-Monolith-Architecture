using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Commands.Plans;
using Base.Domain.Exceptions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using MediatR;

namespace Main.WebApi.Test.Unit.Application.Commands.Plans;

/// <summary>
/// ArchivePlanDocumentCommandHandler 單元測試
/// 測試封存指標計畫文件處理器的各種場景，包括成功封存和異常處理
/// </summary>
public class ArchivePlanDocumentCommandHandlerTests
{
    private readonly ITimeZoneService timeZoneService;
    private readonly IUserService<Scuser> userService;
    private readonly IPlanRepository planRepository;
    private readonly ArchivePlanDocumentCommandHandler handler;

    public ArchivePlanDocumentCommandHandlerTests()
    {
        timeZoneService = Substitute.For<ITimeZoneService>();
        userService = Substitute.For<IUserService<Scuser>>();
        planRepository = Substitute.For<IPlanRepository>();

        handler = new ArchivePlanDocumentCommandHandler(
            timeZoneService,
            userService,
            planRepository);
    }

    #region Valid Request Tests

    [Fact(DisplayName = "處理有效請求_應成功封存計畫文件")]
    public async Task Handle_ValidRequest_ShouldArchivePlanDocumentSuccessfully()
    {
        // Arrange
        DateTime testTime = new(2025, 7, 30, 10, 0, 0);
        const int planDocumentId = 123;
        const string userId = "JasonTsai";

        ArchivePlanDocumentCommand request = new(planDocumentId);
        Scuser mockUser = CreateMockUser(userId);

        timeZoneService.Now.Returns(testTime);
        userService.CurrentNow(Arg.Any<CancellationToken>()).Returns(mockUser);

        // Act
        MediatR.Unit result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(MediatR.Unit.Value, result);
        await planRepository.Received(1).ArchivePlanDocumentAsync(
            planDocumentId,
            testTime,
            userId,
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "處理有效請求_應使用正確的參數呼叫Repository")]
    public async Task Handle_ValidRequest_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        DateTime testTime = new(2025, 7, 30, 14, 30, 0);
        const int planDocumentId = 5929;
        const string userId = "JasonTsai";

        ArchivePlanDocumentCommand request = new(planDocumentId);
        Scuser mockUser = CreateMockUser(userId);
        CancellationToken cancellationToken = new();

        timeZoneService.Now.Returns(testTime);
        userService.CurrentNow(cancellationToken).Returns(mockUser);

        // Act
        await handler.Handle(request, cancellationToken);

        // Assert
        await planRepository.Received(1).ArchivePlanDocumentAsync(
            Arg.Is<int>(id => id == planDocumentId),
            Arg.Is<DateTime>(dt => dt == testTime),
            Arg.Is<string>(u => u == userId),
            Arg.Is<CancellationToken>(ct => ct == cancellationToken));
    }

    #endregion

    #region Exception Handling Tests

    [Fact(DisplayName = "處理請求_當Repository拋出NotFoundException_應傳遞異常")]
    public async Task Handle_WhenRepositoryThrowsNotFoundException_ShouldPropagateException()
    {
        // Arrange
        const int planDocumentId = 5929;
        ArchivePlanDocumentCommand request = new(planDocumentId);
        Scuser mockUser = CreateMockUser("TestUser");

        timeZoneService.Now.Returns(DateTime.UtcNow);
        userService.CurrentNow(Arg.Any<CancellationToken>()).Returns(mockUser);
        planRepository.ArchivePlanDocumentAsync(
            Arg.Any<int>(),
            Arg.Any<DateTime>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Throws(new NotFoundException($"PlanDocument with ID {planDocumentId} not found."));

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(request, CancellationToken.None));

        Assert.Contains(planDocumentId.ToString(), exception.Message);
    }

    [Fact(DisplayName = "處理請求_當Repository拋出一般Exception_應傳遞異常")]
    public async Task Handle_WhenRepositoryThrowsGeneralException_ShouldPropagateException()
    {
        // Arrange
        const int planDocumentId = 789;
        const string errorMessage = "Database connection failed";
        ArchivePlanDocumentCommand request = new(planDocumentId);
        Scuser mockUser = CreateMockUser("TestUser");

        timeZoneService.Now.Returns(DateTime.UtcNow);
        userService.CurrentNow(Arg.Any<CancellationToken>()).Returns(mockUser);
        planRepository.ArchivePlanDocumentAsync(
            Arg.Any<int>(),
            Arg.Any<DateTime>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException(errorMessage));

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(request, CancellationToken.None));

        Assert.Equal(errorMessage, exception.Message);
    }

    #endregion

    #region Service Interaction Tests

    [Fact(DisplayName = "處理請求_應正確呼叫TimeZoneService")]
    public async Task Handle_ShouldCallTimeZoneServiceCorrectly()
    {
        // Arrange
        DateTime expectedTime = new(2025, 7, 30, 16, 45, 30);
        ArchivePlanDocumentCommand request = new(123);
        Scuser mockUser = CreateMockUser("TestUser");

        timeZoneService.Now.Returns(expectedTime);
        userService.CurrentNow(Arg.Any<CancellationToken>()).Returns(mockUser);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        _ = timeZoneService.Received(1).Now;
        await planRepository.Received(1).ArchivePlanDocumentAsync(
            Arg.Any<int>(),
            expectedTime,
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "處理請求_應正確呼叫UserService")]
    public async Task Handle_ShouldCallUserServiceCorrectly()
    {
        // Arrange
        const string expectedUserId = "jason_tsai@Demo.com.tw";
        ArchivePlanDocumentCommand request = new(123);
        Scuser mockUser = CreateMockUser(expectedUserId);
        CancellationToken cancellationToken = new();

        timeZoneService.Now.Returns(DateTime.UtcNow);
        userService.CurrentNow(cancellationToken).Returns(mockUser);

        // Act
        await handler.Handle(request, cancellationToken);

        // Assert
        userService.Received(1).CurrentNow(cancellationToken);
        await planRepository.Received(1).ArchivePlanDocumentAsync(
            Arg.Any<int>(),
            Arg.Any<DateTime>(),
            expectedUserId,
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region Edge Case Tests

    [Fact(DisplayName = "處理請求_使用零值PlanDocumentId_應正常處理")]
    public async Task Handle_WithZeroPlanDocumentId_ShouldHandleNormally()
    {
        // Arrange
        const int planDocumentId = 0;
        ArchivePlanDocumentCommand request = new(planDocumentId);
        Scuser mockUser = CreateMockUser("TestUser");

        timeZoneService.Now.Returns(DateTime.UtcNow);
        userService.CurrentNow(Arg.Any<CancellationToken>()).Returns(mockUser);

        // Act
        MediatR.Unit result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(MediatR.Unit.Value, result);
        await planRepository.Received(1).ArchivePlanDocumentAsync(
            planDocumentId,
            Arg.Any<DateTime>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "處理請求_使用負值PlanDocumentId_應正常處理")]
    public async Task Handle_WithNegativePlanDocumentId_ShouldHandleNormally()
    {
        // Arrange
        const int planDocumentId = -1;
        ArchivePlanDocumentCommand request = new(planDocumentId);
        Scuser mockUser = CreateMockUser("TestUser");

        timeZoneService.Now.Returns(DateTime.UtcNow);
        userService.CurrentNow(Arg.Any<CancellationToken>()).Returns(mockUser);

        // Act
        MediatR.Unit result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(MediatR.Unit.Value, result);
        await planRepository.Received(1).ArchivePlanDocumentAsync(
            planDocumentId,
            Arg.Any<DateTime>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region Cancellation Token Tests

    [Fact(DisplayName = "處理請求_當CancellationToken被取消_應拋出OperationCanceledException")]
    public async Task Handle_WhenCancellationTokenIsCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        ArchivePlanDocumentCommand request = new(123);
        Scuser mockUser = CreateMockUser("TestUser");
        CancellationTokenSource cancellationTokenSource = new();

        timeZoneService.Now.Returns(DateTime.UtcNow);
        userService.CurrentNow(Arg.Any<CancellationToken>()).Returns(mockUser);
        planRepository.ArchivePlanDocumentAsync(
            Arg.Any<int>(),
            Arg.Any<DateTime>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                CancellationToken token = callInfo.Arg<CancellationToken>();
                token.ThrowIfCancellationRequested();
                return Task.CompletedTask;
            });

        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.Handle(request, cancellationTokenSource.Token));
    }

    #endregion

    #region Legacy Test (原有測試，保持向後相容性)

    private ArchivePlanDocumentCommandHandler CreateArchivePlanDocumentCommandHandler()
    {
        timeZoneService.Now.Returns(DateTime.UtcNow);
        userService.CurrentNow(Arg.Any<CancellationToken>()).Returns(new Scuser
        {
            UserId = "test-user-id",
            UserName = "test-user"
        });

        return new ArchivePlanDocumentCommandHandler(
            timeZoneService,
            userService,
            planRepository);
    }

    [Fact]
    public async Task HandleStateUnderTestExpectedBehavior()
    {
        // Arrange
        ArchivePlanDocumentCommandHandler archivePlanDocumentCommandHandler = CreateArchivePlanDocumentCommandHandler();
        ArchivePlanDocumentCommand request = new(1);
        CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(3)); // 模擬超過3秒後取消

        // Act
        MediatR.Unit result = await archivePlanDocumentCommandHandler.Handle(
            request,
            cancellationTokenSource.Token);

        // Assert
        Assert.Equal(MediatR.Unit.Value, result);
        await planRepository.Received(1).ArchivePlanDocumentAsync(
            Arg.Is<int>(x => x == 1),
            Arg.Any<DateTime>(),
            Arg.Is<string>(x => x == "test-user-id"),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 建立模擬使用者物件
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>模擬的Scuser物件</returns>
    private static Scuser CreateMockUser(string userId) => new()
    {
        UserId = userId,
        UserName = $"User_{userId}"
    };

    #endregion
}
