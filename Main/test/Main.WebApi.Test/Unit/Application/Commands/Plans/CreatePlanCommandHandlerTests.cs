using System.Globalization;
using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Commands.Plans;
using Main.WebApi.Application.Queries.CustomTemplate;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Main.WebApi.Test.Unit.Application.Commands.Plans;

/// <summary>
/// CreatePlanCommandHandler 單元測試
/// 測試創建計劃命令處理器的各種場景，包括成功創建和異常處理
/// </summary>
public class CreatePlanCommandHandlerTests(ITestOutputHelper testOutputHelper)
{
    private readonly ILogger<CreatePlanCommandHandler> subLogger = new TestLogger<CreatePlanCommandHandler>(testOutputHelper);
    private readonly ITimeZoneService subTimeZoneService = Substitute.For<ITimeZoneService>();
    private readonly IUserService<Scuser> subUserService = Substitute.For<IUserService<Scuser>>();
    private readonly IPlanRepository subPlanRepository = Substitute.For<IPlanRepository>();
    private readonly ICustomRequestUnitQuery subCustomRequestUnitQuery = Substitute.For<ICustomRequestUnitQuery>();
    private readonly IMediator subMediator = Substitute.For<IMediator>();

    private CreatePlanCommandHandler CreateHandler() => new(
        subLogger,
        subTimeZoneService,
        subUserService,
        subPlanRepository,
        subCustomRequestUnitQuery,
        subMediator);

    [Fact(DisplayName = "成功建立計畫")]
    public async Task HandleValidCommandShouldCreatePlanSuccessfully()
    {
        // Arrange
        CreatePlanCommandHandler handler = CreateHandler();
        DateTime testTime = new(2025, 8, 1, 10, 30, 0);

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            UserName = "測試使用者",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        CreatePlanCommand command = new()
        {
            PlanName = "2025 Demo永續指標計畫",
            Year = 2025,
            CompanyId = 1,
            IndicatorId = "IND001,IND002",
            FactoryId = "F001,F002",
            IndustryId = "GRI,SASB",
            PlanTemplateIdList = [1, 2, 3],
            PlanTemplateVersion = "1.0.0"
        };

        Plan createdPlan = Plan.Create(
            command.PlanName,
            testUser.CurrentTenant.TenantId,
            command.CompanyId,
            command.Year!.Value,
            command.PlanTemplateVersion,
            testTime,
            testUser.UserId);
        createdPlan.PlanId = 100; // 模擬建立後的 ID

        subTimeZoneService.Now.Returns(testTime);
        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanRepository.AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(createdPlan))
            .AndDoes(call => call.Arg<Plan>().PlanId = 100); // 模擬資料庫設定 ID
        subMediator.Send(Arg.Any<CreatePlanDetailCommand>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        int result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(100, result);

        // 驗證 Plan 建立
        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.PlanName == command.PlanName &&
            p.Year == command.Year!.Value.ToString(CultureInfo.InvariantCulture) &&
            p.CompanyId == command.CompanyId &&
            p.TenantId == testUser.CurrentTenant.TenantId &&
            p.PlanTemplateVersion == command.PlanTemplateVersion
        ), Arg.Any<CancellationToken>());

        // 驗證 PlanFactories 建立
        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.PlanFactories.Count == 2 &&
            p.PlanFactories.Any(pf => pf.FactoryId == "F001") &&
            p.PlanFactories.Any(pf => pf.FactoryId == "F002")
        ), Arg.Any<CancellationToken>());

        // 驗證 PlanIndustries 建立
        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.PlanIndustries.Count == 2 &&
            p.PlanIndustries.Any(pi => pi.IndustryId == "GRI") &&
            p.PlanIndustries.Any(pi => pi.IndustryId == "SASB")
        ), Arg.Any<CancellationToken>());

        // 驗證 PlanIndicators 建立
        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.PlanIndicators.Count == 2 &&
            p.PlanIndicators.Any(pi => pi.IndicatorId == "IND001") &&
            p.PlanIndicators.Any(pi => pi.IndicatorId == "IND002")
        ), Arg.Any<CancellationToken>());

        // 驗證 CreatePlanDetail 命令執行次數
        await subMediator.Received(3).Send(Arg.Any<CreatePlanDetailCommand>(), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine($"Test completed: Plan {result} created successfully with {command.PlanTemplateIdList.Length} templates");
    }

    [Fact(DisplayName = "年度為空時應拋出例外")]
    public async Task HandleNullYearShouldThrowHandleException()
    {
        // Arrange
        CreatePlanCommandHandler handler = CreateHandler();

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        CreatePlanCommand command = new()
        {
            PlanName = "測試計畫",
            Year = null, // 年度為空
            CompanyId = 1,
            IndicatorId = "IND001",
            FactoryId = "F001",
            IndustryId = "GRI",
            PlanTemplateIdList = [1]
        };

        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Year is required", exception.Message);

        // 驗證沒有呼叫 Repository
        await subPlanRepository.DidNotReceive().AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine($"Test completed: Exception thrown as expected - {exception.Message}");
    }

    [Fact(DisplayName = "租戶為空時應拋出例外")]
    public async Task HandleNullTenantShouldThrowHandleException()
    {
        // Arrange
        CreatePlanCommandHandler handler = CreateHandler();

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = null! // 租戶為空
        };

        CreatePlanCommand command = new()
        {
            PlanName = "測試計畫",
            Year = 2025,
            CompanyId = 1,
            IndicatorId = "IND001",
            FactoryId = "F001",
            IndustryId = "GRI",
            PlanTemplateIdList = [1]
        };

        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Tenant is required", exception.Message);

        // 驗證沒有呼叫 Repository
        await subPlanRepository.DidNotReceive().AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine($"Test completed: Exception thrown as expected - {exception.Message}");
    }

    [Fact(DisplayName = "空的指標清單仍應成功建立計畫")]
    public async Task HandleEmptyIndicatorListShouldCreatePlanSuccessfully()
    {
        // Arrange
        CreatePlanCommandHandler handler = CreateHandler();
        DateTime testTime = new(2025, 8, 1, 10, 30, 0);

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        CreatePlanCommand command = new()
        {
            PlanName = "最小計畫",
            Year = 2025,
            CompanyId = 1,
            IndicatorId = "", // 空的指標清單
            FactoryId = "",  // 空的工廠清單
            IndustryId = "", // 空的產業清單
            PlanTemplateIdList = [1]
        };

        Plan createdPlan = Plan.Create(
            command.PlanName,
            testUser.CurrentTenant.TenantId,
            command.CompanyId,
            command.Year!.Value,
            command.PlanTemplateVersion,
            testTime,
            testUser.UserId);
        createdPlan.PlanId = 200;

        subTimeZoneService.Now.Returns(testTime);
        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanRepository.AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(createdPlan))
            .AndDoes(call => call.Arg<Plan>().PlanId = 200); // 模擬資料庫設定 ID
        subMediator.Send(Arg.Any<CreatePlanDetailCommand>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        int result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(200, result);

        // 驗證空集合的建立
        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.PlanFactories.Count == 0 &&
            p.PlanIndustries.Count == 0 &&
            p.PlanIndicators.Count == 0
        ), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine("Test completed: Plan created successfully with empty collections");
    }

    [Fact(DisplayName = "包含空白字串的清單應過濾空白項目")]
    public async Task HandleListsWithEmptyStrinDemohouldFilterEmptyItems()
    {
        // Arrange
        CreatePlanCommandHandler handler = CreateHandler();
        DateTime testTime = new(2025, 8, 1, 10, 30, 0);

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        CreatePlanCommand command = new()
        {
            PlanName = "測試過濾計畫",
            Year = 2025,
            CompanyId = 1,
            IndicatorId = "IND001,,IND002,   ,", // 包含空白字串
            FactoryId = "F001, ,F002",            // 包含空白字串
            IndustryId = "GRI,,SASB,,",           // 包含空白字串
            PlanTemplateIdList = [1, 2]
        };

        Plan createdPlan = Plan.Create(
            command.PlanName,
            testUser.CurrentTenant.TenantId,
            command.CompanyId,
            command.Year!.Value,
            command.PlanTemplateVersion,
            testTime,
            testUser.UserId);
        createdPlan.PlanId = 300;

        subTimeZoneService.Now.Returns(testTime);
        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanRepository.AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(createdPlan))
            .AndDoes(call => call.Arg<Plan>().PlanId = 300); // 模擬資料庫設定 ID
        subMediator.Send(Arg.Any<CreatePlanDetailCommand>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        int result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(300, result);

        // 驗證過濾後的結果 - 只有非空白的項目被加入
        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.PlanFactories.Count == 2 &&
            p.PlanFactories.Any(pf => pf.FactoryId == "F001") &&
            p.PlanFactories.Any(pf => pf.FactoryId == "F002")
        ), Arg.Any<CancellationToken>());

        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.PlanIndustries.Count == 2 &&
            p.PlanIndustries.Any(pi => pi.IndustryId == "GRI") &&
            p.PlanIndustries.Any(pi => pi.IndustryId == "SASB")
        ), Arg.Any<CancellationToken>());

        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.PlanIndicators.Count == 2 &&
            p.PlanIndicators.Any(pi => pi.IndicatorId == "IND001") &&
            p.PlanIndicators.Any(pi => pi.IndicatorId == "IND002")
        ), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine("Test completed: Empty strings filtered correctly from all collections");
    }

    [Theory(DisplayName = "不同年度值應正確處理")]
    [InlineData(2020, "2020")]
    [InlineData(2025, "2025")]
    [InlineData(2030, "2030")]
    public async Task HandleDifferentYearValuesShouldHandleCorrectly(int year, string expectedYearString)
    {
        // Arrange
        CreatePlanCommandHandler handler = CreateHandler();
        DateTime testTime = new(2025, 8, 1, 10, 30, 0);

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        CreatePlanCommand command = new()
        {
            PlanName = $"{year} 年度計畫",
            Year = year,
            CompanyId = 1,
            IndicatorId = "IND001",
            FactoryId = "F001",
            IndustryId = "GRI",
            PlanTemplateIdList = [1]
        };

        Plan createdPlan = Plan.Create(
            command.PlanName,
            testUser.CurrentTenant.TenantId,
            command.CompanyId,
            command.Year!.Value,
            command.PlanTemplateVersion,
            testTime,
            testUser.UserId);
        createdPlan.PlanId = year; // 使用年度作為 ID

        subTimeZoneService.Now.Returns(testTime);
        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanRepository.AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(createdPlan))
            .AndDoes(call => call.Arg<Plan>().PlanId = year); // 模擬資料庫設定 ID
        subMediator.Send(Arg.Any<CreatePlanDetailCommand>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        int result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(year, result);

        await subPlanRepository.Received(1).AddAsync(Arg.Is<Plan>(p =>
            p.Year == expectedYearString
        ), Arg.Any<CancellationToken>());

        testOutputHelper.WriteLine($"Test completed: Year {year} handled correctly as string '{expectedYearString}'");
    }

    [Fact(DisplayName = "CreatePlanDetail 命令應包含正確的參數")]
    public async Task HandleValidCommandShouldSendCorrectCreatePlanDetailCommands()
    {
        // Arrange
        CreatePlanCommandHandler handler = CreateHandler();
        DateTime testTime = new(2025, 8, 1, 10, 30, 0);

        Scuser testUser = new()
        {
            UserId = "test-user-01",
            CurrentTenant = new() { TenantId = "tenant-001" }
        };

        CreatePlanCommand command = new()
        {
            PlanName = "詳細測試計畫",
            Year = 2025,
            CompanyId = 1,
            IndicatorId = "IND001",
            FactoryId = "F001",
            IndustryId = "GRI",
            PlanTemplateIdList = [10, 20, 30] // 三個範本
        };

        Plan createdPlan = Plan.Create(
            command.PlanName,
            testUser.CurrentTenant.TenantId,
            command.CompanyId,
            command.Year!.Value,
            command.PlanTemplateVersion,
            testTime,
            testUser.UserId);
        createdPlan.PlanId = 500;

        subTimeZoneService.Now.Returns(testTime);
        subUserService.Now(Arg.Any<CancellationToken>()).Returns(testUser);
        subPlanRepository.AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(createdPlan))
            .AndDoes(call => call.Arg<Plan>().PlanId = 500); // 模擬資料庫設定 ID
        subMediator.Send(Arg.Any<CreatePlanDetailCommand>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        int result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(500, result);

        // 驗證每個 PlanTemplate 都有對應的 CreatePlanDetailCommand
        foreach (int templateId in command.PlanTemplateIdList)
        {
            await subMediator.Received(1).Send(Arg.Is<CreatePlanDetailCommand>(cmd =>
                cmd.Year == command.Year &&
                cmd.PlanTemplateId == templateId &&
                cmd.PlanId == createdPlan.PlanId &&
                cmd.RowNumber >= 1
            ), Arg.Any<CancellationToken>());
        }

        testOutputHelper.WriteLine($"Test completed: {command.PlanTemplateIdList.Length} CreatePlanDetailCommand sent with correct parameters");
    }
}
