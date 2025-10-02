using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Commands.Plans;
using Main.WebApi.Application.Queries.Plans;
using Main.WebApi.Application.Queries.PlanTemplates;
using Main.WebApi.Application.Queries.CustomTemplate;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Main.WebApi.Test.Unit.Application.Commands.Plans;

/// <summary>
/// CreatePlanDetailCommandHandler 單元測試
/// 測試建立計畫詳細資料命令處理器的各種場景，包括成功建立和異常處理
/// </summary>
public class CreatePlanDetailCommandHandlerTests(ITestOutputHelper testOutputHelper)
{
    private readonly ILogger<CreatePlanDetailCommandHandler> subLogger = new TestLogger<CreatePlanDetailCommandHandler>(testOutputHelper);
    private readonly IPlanQuery subPlanQuery = Substitute.For<IPlanQuery>();
    private readonly IPlanTemplateQuery subPlanTemplateQuery = Substitute.For<IPlanTemplateQuery>();
    private readonly ICustomPlanTemplateQuery subCustomPlanTemplateQuery = Substitute.For<ICustomPlanTemplateQuery>();
    private readonly IPlanRepository subPlanRepository = Substitute.For<IPlanRepository>();
    private readonly IUserService<Scuser> subUserService = Substitute.For<IUserService<Scuser>>();

    private CreatePlanDetailCommandHandler CreateHandler() => new(
        subLogger,
        subPlanQuery,
        subPlanTemplateQuery,
        subCustomPlanTemplateQuery,
        subPlanRepository,
        subUserService);

    [Fact(DisplayName = "成功建立計畫詳細資料")]
    public async Task HandleValidCommandShouldCreatePlanDetailSuccessfully()
    {
        // Arrange
        CreatePlanDetailCommandHandler handler = CreateHandler();

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            UserName = "測試使用者",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        Plan testPlan = new()
        {
            PlanId = 100,
            PlanName = "測試計畫",
            CompanyId = 1,
            TenantId = "tenant-001",
            Year = "2025"
        };

        PlanTemplate testPlanTemplate = new()
        {
            PlanTemplateId = 1,
            PlanTemplateName = "測試計畫範本",
            PlanTemplateEnName = "Test Plan Template",
            PlanTemplateChName = "測試計畫範本",
            PlanTemplateJpName = "テスト計画テンプレート",
            AcceptDataSource = "API",
            GroupId = "TEST_GROUP",
            CycleType = "month",
            PlanTemplateForms = [
                new() { TenantId = "tenant-001", FormId = 12345 }
            ]
        };

        CreatePlanDetailCommand command = new()
        {
            PlanId = 100,
            PlanTemplateId = 1,
            Year = 2025,
            RowNumber = 1
        };

        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanQuery.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(testPlan);
        subPlanTemplateQuery.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(testPlanTemplate);

        // Act
        bool result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        await subPlanRepository.Received(1).AddPlanDetailAsync(
            Arg.Any<PlanDetail>(),
            Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine("測試完成：成功建立計畫詳細資料");
    }

    [Fact(DisplayName = "PlanId為null時應拋出HandleException")]
    public async Task HandleNullPlanIdShouldThrowHandleException()
    {
        // Arrange
        CreatePlanDetailCommandHandler handler = CreateHandler();

        CreatePlanDetailCommand command = new()
        {
            PlanId = null,
            PlanTemplateId = 1,
            Year = 2025,
            RowNumber = 1
        };

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("PlanId is not found", exception.Message);
        testOutputHelper.WriteLine("測試完成：PlanId為null時正確拋出異常");
    }

    [Fact(DisplayName = "PlanTemplateId為null時應拋出HandleException")]
    public async Task HandleNullPlanTemplateIdShouldThrowHandleException()
    {
        // Arrange
        CreatePlanDetailCommandHandler handler = CreateHandler();

        CreatePlanDetailCommand command = new()
        {
            PlanId = 100,
            PlanTemplateId = null,
            Year = 2025,
            RowNumber = 1
        };

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("PlanTemplateId is not found", exception.Message);
        testOutputHelper.WriteLine("測試完成：PlanTemplateId為null時正確拋出異常");
    }

    [Fact(DisplayName = "計畫不存在時應拋出HandleException")]
    public async Task HandlePlanNotFoundShouldThrowHandleException()
    {
        // Arrange
        CreatePlanDetailCommandHandler handler = CreateHandler();

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        CreatePlanDetailCommand command = new()
        {
            PlanId = 999,
            PlanTemplateId = 1,
            Year = 2025,
            RowNumber = 1
        };

        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanQuery.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Plan?)null);

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Plan not found", exception.Message);
        testOutputHelper.WriteLine("測試完成：計畫不存在時正確拋出異常");
    }

    [Fact(DisplayName = "計畫範本不存在時應拋出HandleException")]
    public async Task HandlePlanTemplateNotFoundShouldThrowHandleException()
    {
        // Arrange
        CreatePlanDetailCommandHandler handler = CreateHandler();

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        Plan testPlan = new()
        {
            PlanId = 100,
            PlanName = "測試計畫",
            CompanyId = 1,
            TenantId = "tenant-001",
            Year = "2025"
        };

        CreatePlanDetailCommand command = new()
        {
            PlanId = 100,
            PlanTemplateId = 999,
            Year = 2025,
            RowNumber = 1
        };

        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanQuery.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(testPlan);
        subPlanTemplateQuery.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((PlanTemplate?)null);

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("PlanTemplate not found", exception.Message);
        testOutputHelper.WriteLine("測試完成：計畫範本不存在時正確拋出異常");
    }

    [Fact(DisplayName = "找不到對應租戶的表單時應拋出HandleException")]
    public async Task HandleFormNotFoundForTenantShouldThrowHandleException()
    {
        // Arrange
        CreatePlanDetailCommandHandler handler = CreateHandler();

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-999" }
        };

        Plan testPlan = new()
        {
            PlanId = 100,
            PlanName = "測試計畫",
            CompanyId = 1,
            TenantId = "tenant-999",
            Year = "2025"
        };

        PlanTemplate testPlanTemplate = new()
        {
            PlanTemplateId = 1,
            PlanTemplateName = "測試計畫範本",
            PlanTemplateForms = [
                new() { TenantId = "tenant-001", FormId = 12345 }
            ]
        };

        CreatePlanDetailCommand command = new()
        {
            PlanId = 100,
            PlanTemplateId = 1,
            Year = 2025,
            RowNumber = 1
        };

        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanQuery.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(testPlan);
        subPlanTemplateQuery.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(testPlanTemplate);

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Form not found", exception.Message);
        testOutputHelper.WriteLine("測試完成：找不到對應租戶的表單時正確拋出異常");
    }

    [Theory(DisplayName = "使用預設值建立計畫詳細資料")]
    [InlineData("quarter", true)]
    [InlineData(null, true)]
    public async Task HandleUsesDefaultValuesWhenTemplateValuesAreNullOrEmpty(string? cycleType, bool expectedCycleMonthLast)
    {
        // Arrange
        CreatePlanDetailCommandHandler handler = CreateHandler();

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        Plan testPlan = new()
        {
            PlanId = 100,
            PlanName = "測試計畫",
            CompanyId = 1,
            TenantId = "tenant-001",
            Year = "2025"
        };

        PlanTemplate testPlanTemplate = new()
        {
            PlanTemplateId = 1,
            PlanTemplateName = "測試計畫範本",
            CycleType = cycleType,
            GroupId = "TEST_GROUP",
            PlanTemplateForms = [
                new() { TenantId = "tenant-001", FormId = 12345 }
            ]
        };

        CreatePlanDetailCommand command = new()
        {
            PlanId = 100,
            PlanTemplateId = 1,
            Year = 2025,
            RowNumber = 1
        };

        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanQuery.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(testPlan);
        subPlanTemplateQuery.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(testPlanTemplate);

        string expectedCycleType = cycleType ?? "year";

        // Act
        bool result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        await subPlanRepository.Received(1).AddPlanDetailAsync(
            Arg.Any<PlanDetail>(),
            Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine($"測試完成：CycleType={expectedCycleType}, CycleMonthLast={expectedCycleMonthLast}");
    }

    [Fact(DisplayName = "驗證建立的PlanDetail包含正確的中繼資料")]
    public async Task HandleSetsCorrectMetadataOnCreatedPlanDetail()
    {
        // Arrange
        CreatePlanDetailCommandHandler handler = CreateHandler();

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        Plan testPlan = new()
        {
            PlanId = 100,
            PlanName = "測試計畫",
            CompanyId = 1,
            TenantId = "tenant-001",
            Year = "2025"
        };

        PlanTemplate testPlanTemplate = new()
        {
            PlanTemplateId = 1,
            PlanTemplateName = "測試計畫範本",
            PlanTemplateForms = [
                new() { TenantId = "tenant-001", FormId = 12345 }
            ]
        };

        CreatePlanDetailCommand command = new()
        {
            PlanId = 100,
            PlanTemplateId = 1,
            Year = 2025,
            RowNumber = 1
        };

        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanQuery.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(testPlan);
        subPlanTemplateQuery.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(testPlanTemplate);

        // Act
        bool result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        await subPlanRepository.Received(1).AddPlanDetailAsync(
            Arg.Any<PlanDetail>(),
            Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine("測試完成：驗證建立的PlanDetail包含正確的中繼資料");
    }
}
