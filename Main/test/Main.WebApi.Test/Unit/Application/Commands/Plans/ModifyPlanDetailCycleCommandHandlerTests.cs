using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.WebApi.Application.Commands.Plans;
using Main.WebApi.Application.Queries.Plans;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Main.WebApi.Test.Unit.Application.Commands.Plans;

/// <summary>
/// ModifyPlanDetailCycleCommandHandler 單元測試類別
/// </summary>
public class ModifyPlanDetailCycleCommandHandlerTests(ITestOutputHelper testOutputHelper)
{
    private readonly ILogger<ModifyPlanDetailCycleCommandHandler> subLogger = new TestLogger<ModifyPlanDetailCycleCommandHandler>(testOutputHelper);
    private readonly IMediator subMediator = Substitute.For<IMediator>();
    private readonly ITimeZoneService subTimeZoneService = Substitute.For<ITimeZoneService>();
    private readonly IPlanDetailQuery subPlanDetailQuery = Substitute.For<IPlanDetailQuery>();
    private readonly IPlanDocumentQuery subPlanDocumentQuery = Substitute.For<IPlanDocumentQuery>();
    private readonly IPlanRepository subPlanRepository = Substitute.For<IPlanRepository>();

    private ModifyPlanDetailCycleCommandHandler CreateHandler() => new(
            subLogger,
            subMediator,
            subTimeZoneService,
            subPlanDetailQuery,
            subPlanDocumentQuery,
            subPlanRepository);

    [Fact(DisplayName = "成功修改計畫詳細資料週期")]
    public async Task HandleValidCommandShouldSucceed()
    {
        // Arrange
        ModifyPlanDetailCycleCommandHandler handler = CreateHandler();

        ModifyPlanDetailCycleCommand command = new()
        {
            PlanDetailId = 1,
            CycleType = "year",
            CycleMonth = null,
            CycleDay = null,
            CycleMonthLast = false,
            EndDate = new DateTime(2026, 11, 30, 23, 59, 59),
            IsApplyAll = false,
            PlanDocumentCycleArray = []
        };

        // 建立真實的 PlanDetail 物件而不是 Mock
        PlanDetail planDetail = new()
        {
            PlanDetailId = 1,
            PlanId = 100,
            PlanTemplateId = 1,
            PlanDetailName = "Test Plan Detail",
            GroupId = "TestGroup",
            CycleType = "month"  // 設定初始的 CycleType 與命令中的不同
        };

        List<PlanDocument> planDocuments = [
            new(1, DateTime.Today, DateTime.Today.AddDays(30), 1, 2024, 4, 12, "user1", "user1")
        ];

        subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(planDetail);

        subPlanDocumentQuery.ListByDetailIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(planDocuments);

        // Act
        bool result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        // 驗證方法呼叫順序
        Received.InOrder(() =>
        {
            subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>());
            subPlanRepository.UpdatePlanDetailAsync(planDetail, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
            subPlanDocumentQuery.ListByDetailIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>());
        });

        // 驗證 PlanDetail 的週期類型已被修改
        Assert.Equal(command.CycleType, planDetail.CycleType);
        Assert.Equal(command.EndDate, planDetail.EndDate);

        testOutputHelper.WriteLine($"Test completed successfully: Plan detail {command.PlanDetailId} cycle changed to {command.CycleType}");
    }

    [Fact(DisplayName = "找不到計畫詳細資料時應拋出例外")]
    public async Task HandlePlanDetailNotFoundShouldThrowException()
    {
        // Arrange
        ModifyPlanDetailCycleCommandHandler handler = CreateHandler();

        ModifyPlanDetailCycleCommand command = new()
        {
            PlanDetailId = 999,
            CycleType = "month",
            CycleMonth = 1,
            CycleDay = 1,
            CycleMonthLast = false,
            EndDate = new DateTime(2025, 12, 31),
            IsApplyAll = false,
            PlanDocumentCycleArray = []
        };

        subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns((PlanDetail?)null);

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await handler.Handle(command, CancellationToken.None));

        await subPlanDetailQuery.Received(1).GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>());
        await subPlanDocumentQuery.DidNotReceive().ListByDetailIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await subPlanRepository.DidNotReceive().UpdatePlanDetailAsync(Arg.Any<PlanDetail>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine($"Test completed: Exception thrown as expected for non-existent plan detail: {exception.Message}");
    }

    [Fact(DisplayName = "空的計畫文件清單仍應成功更新週期")]
    public async Task HandleEmptyPlanDocumentsShouldSucceed()
    {
        // Arrange
        ModifyPlanDetailCycleCommandHandler handler = CreateHandler();

        ModifyPlanDetailCycleCommand command = new()
        {
            PlanDetailId = 1,
            CycleType = "year",
            CycleMonth = 12,
            CycleDay = null,
            CycleMonthLast = true,
            EndDate = new DateTime(2025, 12, 31),
            IsApplyAll = false,
            PlanDocumentCycleArray = []
        };

        PlanDetail planDetail = new()
        {
            PlanDetailId = 1,
            PlanId = 100,
            PlanTemplateId = 1,
            PlanDetailName = "Test Plan Detail",
            GroupId = "TestGroup",
            CycleType = "month"  // 設定初始的 CycleType 與命令中的不同
        };

        List<PlanDocument> emptyPlanDocuments = [];

        subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(planDetail);

        subPlanDocumentQuery.ListByDetailIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(emptyPlanDocuments);

        // Act
        bool result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        await subPlanRepository.Received(1).UpdatePlanDetailAsync(planDetail, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());

        // 驗證週期類型已被修改
        Assert.Equal(command.CycleType, planDetail.CycleType);
        Assert.Equal(command.EndDate, planDetail.EndDate);

        testOutputHelper.WriteLine("Test completed: Successfully updated cycle even with no plan documents");
    }

    [Fact(DisplayName = "當IsApplyAll為true時應成功修改所有相同週期類型的計畫詳細資料")]
    public async Task HandleIsApplyAllTrueShouldModifyAllSameCycleTypeDetails()
    {
        // Arrange
        ModifyPlanDetailCycleCommandHandler handler = CreateHandler();

        ModifyPlanDetailCycleCommand command = new()
        {
            PlanDetailId = 1,
            CycleType = "quarter",
            CycleMonth = 3,
            CycleDay = 15,
            CycleMonthLast = false,
            EndDate = new DateTime(2025, 12, 31),
            IsApplyAll = true,
            PlanDocumentCycleArray = [
                new PlanDocumentCycle { Quarter = 1, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 3, 31) },
                new PlanDocumentCycle { Quarter = 2, StartDate = new DateTime(2025, 4, 1), EndDate = new DateTime(2025, 6, 30) }
            ]
        };

        PlanDetail targetPlanDetail = new()
        {
            PlanDetailId = 1,
            PlanId = 100,
            PlanTemplateId = 1,
            PlanDetailName = "Target Plan Detail",
            GroupId = "TestGroup",
            CycleType = "month"
        };

        // 建立多個 PlanDetail，包含相同和不同的 CycleType
        List<PlanDetail> allPlanDetails = [
            targetPlanDetail,
            new PlanDetail
            {
                PlanDetailId = 2,
                PlanId = 100,
                PlanTemplateId = 2,
                PlanDetailName = "Same Cycle Plan Detail",
                GroupId = "TestGroup",
                CycleType = "quarter", // 與命令相同的 CycleType
                PlanDocuments = [
                    new PlanDocument(2, DateTime.Today, DateTime.Today.AddDays(90), 1, 2025, 1, null, "user1", "user1")
                ]
            },
            new PlanDetail
            {
                PlanDetailId = 3,
                PlanId = 100,
                PlanTemplateId = 3,
                PlanDetailName = "Different Cycle Plan Detail",
                GroupId = "TestGroup",
                CycleType = "year" // 不同的 CycleType，應該跳過
            }
        ];

        List<PlanDocument> targetPlanDocuments = [
            new PlanDocument(1, DateTime.Today, DateTime.Today.AddDays(30), 1, 2025, 1, 1, "user1", "user1")
        ];

        subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(targetPlanDetail);

        subPlanDocumentQuery.ListByDetailIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(targetPlanDocuments);

        subPlanDetailQuery.ListByPlanIdAsync(targetPlanDetail.PlanId, Arg.Any<CancellationToken>())
            .Returns(allPlanDetails);

        // Act
        bool result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        // 驗證目標 PlanDetail 被更新
        await subPlanRepository.Received(1).UpdatePlanDetailAsync(targetPlanDetail, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());

        // 驗證相同 CycleType 的 PlanDetail 也被更新（PlanDetailId = 2）
        await subPlanRepository.Received(1).UpdatePlanDetailAsync(
            Arg.Is<PlanDetail>(x => x.PlanDetailId == 2),
            Arg.Any<DateTime>(),
            Arg.Any<CancellationToken>());

        // 驗證不同 CycleType 的 PlanDetail 沒有被更新（只會更新兩次，不包含 PlanDetailId = 3）
        await subPlanRepository.Received(2).UpdatePlanDetailAsync(Arg.Any<PlanDetail>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());

        // 驗證 PlanDocument 被更新
        await subPlanRepository.Received(1).UpdatePlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());

        // 驗證 Mediator 被呼叫用於封存 PlanDocument
        await subMediator.Received(1).Send(Arg.Any<ArchivePlanDocumentCommand>(), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine($"Test completed: IsApplyAll=true processed {allPlanDetails.Count} plan details");
    }

    [Theory(DisplayName = "GetPlanDocumentCycle方法應正確返回對應的週期資料")]
    [InlineData("year", 2025, null, null, true)]
    [InlineData("quarter", null, 2, null, true)]
    [InlineData("month", null, null, 3, true)]
    [InlineData("invalid", 2025, null, null, false)]
    public async Task GetPlanDocumentCycleShouldReturnCorrectCycle(string cycleType, int? year, int? quarter, int? month, bool shouldFind)
    {
        // Arrange
        ModifyPlanDetailCycleCommandHandler handler = CreateHandler();

        List<PlanDocumentCycle> cycles = [
            new PlanDocumentCycle { Year = 2025, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31) },
            new PlanDocumentCycle { Quarter = 2, StartDate = new DateTime(2025, 4, 1), EndDate = new DateTime(2025, 6, 30) },
            new PlanDocumentCycle { Month = 3, StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2025, 3, 31) }
        ];

        ModifyPlanDetailCycleCommand command = new()
        {
            PlanDetailId = 1,
            CycleType = cycleType,
            CycleMonth = null,
            CycleDay = null,
            CycleMonthLast = false,
            EndDate = new DateTime(2025, 12, 31),
            IsApplyAll = true,
            PlanDocumentCycleArray = cycles
        };

        PlanDetail planDetail = new()
        {
            PlanDetailId = 1,
            PlanId = 100,
            CycleType = "month",
            PlanDetailName = "Test Plan Detail",
            GroupId = "TEST_GROUP"
        };

        List<PlanDetail> allPlanDetails = [
            planDetail,
            new PlanDetail
            {
                PlanDetailId = 2,
                PlanId = 100,
                CycleType = cycleType,
                PlanDetailName = "Another Plan Detail",
                GroupId = "TEST_GROUP",
                PlanDocuments = [
                    new PlanDocument(2, DateTime.Today, DateTime.Today.AddDays(30), 1, year, quarter, month, "user1", "user1")
                ]
            }
        ];

        subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(planDetail);

        subPlanDocumentQuery.ListByDetailIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(new List<PlanDocument>());

        subPlanDetailQuery.ListByPlanIdAsync(planDetail.PlanId, Arg.Any<CancellationToken>())
            .Returns(allPlanDetails);

        // Act & Assert
        if (shouldFind)
        {
            bool result = await handler.Handle(command, CancellationToken.None);
            Assert.True(result);

            // 如果應該找到對應的週期，驗證 PlanDocument 被更新
            if (cycleType != "invalid")
            {
                await subPlanRepository.Received().UpdatePlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
            }
        }
        else
        {
            // 無效的 cycleType 應該拋出 ArgumentException
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await handler.Handle(command, CancellationToken.None));

            Assert.Contains("Invalid cycle type", exception.Message);
        }

        testOutputHelper.WriteLine($"Test completed for cycle type: {cycleType}, should find: {shouldFind}");
    }

    [Fact(DisplayName = "當PlanDocumentCycle為null時應跳過PlanDocument更新")]
    public async Task HandleNullPlanDocumentCycleShouldSkipUpdate()
    {
        // Arrange
        ModifyPlanDetailCycleCommandHandler handler = CreateHandler();

        ModifyPlanDetailCycleCommand command = new()
        {
            PlanDetailId = 1,
            CycleType = "month",
            CycleMonth = null,
            CycleDay = 15,
            CycleMonthLast = false,
            EndDate = new DateTime(2025, 12, 31),
            IsApplyAll = true,
            PlanDocumentCycleArray = [] // 空的週期陣列，將會找不到對應的週期
        };

        PlanDetail planDetail = new()
        {
            PlanDetailId = 1,
            PlanId = 100,
            CycleType = "year",
            PlanDetailName = "Test Plan Detail for Null Cycle",
            GroupId = "TEST_GROUP"
        };

        List<PlanDetail> allPlanDetails = [
            planDetail,
            new PlanDetail
            {
                PlanDetailId = 2,
                PlanId = 100,
                CycleType = "month",
                PlanDetailName = "Another Plan Detail",
                GroupId = "TEST_GROUP",
                PlanDocuments = [
                    new PlanDocument(2, DateTime.Today, DateTime.Today.AddDays(30), 1, 2025, null, 3, "user1", "user1")
                ]
            }
        ];

        subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(planDetail);

        subPlanDocumentQuery.ListByDetailIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns([]);

        subPlanDetailQuery.ListByPlanIdAsync(planDetail.PlanId, Arg.Any<CancellationToken>())
            .Returns(allPlanDetails);

        // Act
        bool result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        // 驗證 PlanDetail 被更新了兩次（目標 + 相同週期類型）
        await subPlanRepository.Received(2).UpdatePlanDetailAsync(Arg.Any<PlanDetail>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());

        // 驗證 PlanDocument 沒有被更新（因為找不到對應的週期）
        await subPlanRepository.DidNotReceive().UpdatePlanDocumentAsync(Arg.Any<PlanDocument>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine("Test completed: Null PlanDocumentCycle correctly skipped PlanDocument update");
    }

    [Fact(DisplayName = "當CancellationToken被取消時應拋出OperationCanceledException")]
    public async Task HandleCancelledTokenShouldThrowOperationCanceledException()
    {
        // Arrange
        ModifyPlanDetailCycleCommandHandler handler = CreateHandler();

        ModifyPlanDetailCycleCommand command = new()
        {
            PlanDetailId = 1,
            CycleType = "year",
            CycleMonth = null,
            CycleDay = null,
            CycleMonthLast = false,
            EndDate = new DateTime(2025, 12, 31),
            IsApplyAll = false,
            PlanDocumentCycleArray = []
        };

        CancellationTokenSource cts = new();
        cts.Cancel(); // 立即取消

        subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(Task.FromCanceled<PlanDetail?>(cts.Token));

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await handler.Handle(command, cts.Token));

        testOutputHelper.WriteLine("Test completed: OperationCanceledException correctly thrown for cancelled token");
    }

    [Theory(DisplayName = "不同的週期類型應該正確更新")]
    [InlineData("year", null, null, false)]
    [InlineData("quarter", 3, null, true)]
    [InlineData("month", null, 15, false)]
    public async Task HandleDifferentCycleTypesShouldUpdateCorrectly(
        string cycleType, int? cycleMonth, int? cycleDay, bool cycleMonthLast)
    {
        // Arrange
        ModifyPlanDetailCycleCommandHandler handler = CreateHandler();

        ModifyPlanDetailCycleCommand command = new()
        {
            PlanDetailId = 1,
            CycleType = cycleType,
            CycleMonth = cycleMonth,
            CycleDay = cycleDay,
            CycleMonthLast = cycleMonthLast,
            EndDate = new DateTime(2025, 12, 31),
            IsApplyAll = false,
            PlanDocumentCycleArray = []
        };

        PlanDetail planDetail = new()
        {
            PlanDetailId = 1,
            PlanId = 100,
            PlanTemplateId = 1,
            PlanDetailName = "Test Plan Detail",
            GroupId = "TestGroup",
            CycleType = "month"  // 不同的初始值
        };

        subPlanDetailQuery.GetByIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns(planDetail);

        subPlanDocumentQuery.ListByDetailIdAsync(command.PlanDetailId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        bool result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(cycleType, planDetail.CycleType);
        await subPlanRepository.Received(1).UpdatePlanDetailAsync(planDetail, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine($"Test completed: Cycle type {cycleType} updated successfully");
    }
}
