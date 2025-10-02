using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Commands.Plans;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Main.WebApi.Test.Unit.Application.Commands.Plans;

/// <summary>
/// AttachDocument2PlanDecumentCommandHandler 單元測試
/// 測試將文件附加到計劃文件命令處理器的各種場景，包括建立新的計劃文件並附加文件
/// </summary>
public class AttachDocument2PlanDecumentCommandHandlerTests(ITestOutputHelper testOutputHelper)
{
    private readonly ILogger<AttachDocument2PlanDecumentCommandHandler> subLogger = new TestLogger<AttachDocument2PlanDecumentCommandHandler>(testOutputHelper);
    private readonly IUserService<Scuser> subUserService = Substitute.For<IUserService<Scuser>>();
    private readonly ITimeZoneService subTimeZoneService = Substitute.For<ITimeZoneService>();
    private readonly IPlanRepository subPlanRepository = Substitute.For<IPlanRepository>();

    private AttachDocument2PlanDecumentCommandHandler CreateHandler() => new(
        subLogger,
        subUserService,
        subTimeZoneService,
        subPlanRepository);

    [Fact(DisplayName = "成功建立新的計劃文件並附加文件")]
    public async Task HandleValidCommandShouldCreatePlanDocumentAndAttachDocument()
    {
        // Arrange
        AttachDocument2PlanDecumentCommandHandler handler = CreateHandler();
        CancellationToken cancellationToken = CancellationToken.None;

        AttachDocument2PlanDecumentCommand command = new()
        {
            PlanDetailId = 1,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            Year = 2025,
            Quarter = null,
            Month = null,
            DocumentId = 342,
            planFactoryId = 1
        };

        Scuser testUser = new()
        {
            UserId = "test-user-01"
        };

        PlanDocument newPlanDocument = new(
            planDetailId: 1,
            startDate: new DateTime(2025, 1, 1),
            endDate: new DateTime(2025, 12, 31),
            planFactoryId: 1,
            year: 2025,
            quarter: null,
            month: null,
            createdUser: "test-user-01",
            modifiedUser: "test-user-01"
        )
        {
            PlanDocumentId = 100
        };

        subUserService.Now(cancellationToken).Returns(testUser);
        subPlanRepository.AddPlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<CancellationToken>())
            .Returns(newPlanDocument);
        subTimeZoneService.Now.Returns(DateTime.UtcNow);

        // Act
        bool result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result);
        await subPlanRepository.Received(1).AddPlanDocumentAsync(Arg.Any<PlanDocument>(), cancellationToken);
        await subPlanRepository.Received(1).UpdatePlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<DateTime>(), cancellationToken);
    }

    [Fact(DisplayName = "測試不同文件ID的處理")]
    public async Task HandleDifferentDocumentIdsShouldProcessCorrectly()
    {
        // Arrange
        AttachDocument2PlanDecumentCommandHandler handler = CreateHandler();
        CancellationToken cancellationToken = CancellationToken.None;

        AttachDocument2PlanDecumentCommand command = new()
        {
            PlanDetailId = 1,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            Year = 2025,
            DocumentId = 999,
            planFactoryId = 1
        };

        Scuser testUser = new() { UserId = "test-user-01" };
        PlanDocument newPlanDocument = new(
            planDetailId: 1,
            startDate: DateTime.Today,
            endDate: DateTime.Today.AddDays(30),
            planFactoryId: 1,
            year: 2025,
            quarter: null,
            month: null,
            createdUser: "test-user-01",
            modifiedUser: "test-user-01")
        {
            PlanDocumentId = 1
        };

        subUserService.Now(cancellationToken).Returns(testUser);
        subPlanRepository.AddPlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<CancellationToken>()).Returns(newPlanDocument);
        subTimeZoneService.Now.Returns(DateTime.UtcNow);

        // Act
        bool result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result);
        await subPlanRepository.Received(1).AddPlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<CancellationToken>());
        await subPlanRepository.Received(1).UpdatePlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "驗證使用者服務正確呼叫")]
    public async Task HandleValidCommandShouldCallUserServiceCorrectly()
    {
        // Arrange
        AttachDocument2PlanDecumentCommandHandler handler = CreateHandler();
        CancellationToken cancellationToken = CancellationToken.None;

        AttachDocument2PlanDecumentCommand command = new()
        {
            PlanDetailId = 1,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            DocumentId = 100,
            planFactoryId = 1
        };

        Scuser testUser = new() { UserId = "admin-user" };
        PlanDocument newPlanDocument = new(
            planDetailId: 1,
            startDate: DateTime.Today,
            endDate: DateTime.Today.AddDays(30),
            planFactoryId: 1,
            year: 2025,
            quarter: null,
            month: null,
            createdUser: "admin-user",
            modifiedUser: "admin-user")
        {
            PlanDocumentId = 50
        };

        subUserService.Now(cancellationToken).Returns(testUser);
        subPlanRepository.AddPlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<CancellationToken>()).Returns(newPlanDocument);
        subTimeZoneService.Now.Returns(DateTime.UtcNow);

        // Act
        bool result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result);
        await subUserService.Received(1).Now(cancellationToken);
    }
}
