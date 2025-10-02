# Csharp xUnit Test Sample

```csharp
using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Commands.Plans;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Globalization;
using Xunit.Abstractions;

namespace Main.WebApi.Test.Unit.Application.Commands.Plans;

/// <summary>
/// CreatePlanCommandHandler 單元測試
/// 測試創建計劃命令處理器的各種場景，包括成功創建和異常處理
/// </summary>
public class CreatePlanCommandHandlerTests
{
    private readonly ILogger<CreatePlanCommandHandler> logger;
    private readonly ITimeZoneService timeZoneService;
    private readonly IUserService<Scuser> userService;
    private readonly IPlanRepository planRepository;
    private readonly IMediator mediator;
    private readonly CreatePlanCommandHandler handler;

    public CreatePlanCommandHandlerTests(ITestOutputHelper output)
    {
        logger = Substitute.For<TestLogger<CreatePlanCommandHandler>>(output);
        timeZoneService = Substitute.For<ITimeZoneService>();
        userService = Substitute.For<IUserService<Scuser>>();
        planRepository = Substitute.For<IPlanRepository>();
        mediator = Substitute.For<IMediator>();

        handler = new CreatePlanCommandHandler(
            logger,
            timeZoneService,
            userService,
            planRepository,
            mediator);
    }

    #region Valid Request Tests

    [Fact(DisplayName = "處理有效請求_應成功創建計劃")]
    public async Task HandleValidRequestShouldCreatePlanSuccessfully()
    {
        // Arrange
        DateTime testTime = new(2025, 1, 1, 10, 0, 0);
        CreatePlanCommand request = CreateValidCommand();
        Scuser mockUser = CreateMockUser();
        Plan mockPlan = CreateMockPlan();

        timeZoneService.Now.Returns(testTime);
        userService.Now(Arg.Any<CancellationToken>()).Returns(mockUser);
        planRepository.AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>())
            .Returns(mockPlan);

        // Act
        int result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(mockPlan.PlanId, result);
        await planRepository.Received(1).AddAsync(Arg.Any<Plan>(), Arg.Any<CancellationToken>());
        await planRepository.Received(2).AddPlanFactoryAsync(Arg.Any<PlanFactory>(), Arg.Any<CancellationToken>());
        await planRepository.Received(1).AddPlanIndustryAsync(Arg.Any<PlanIndustry>(), Arg.Any<CancellationToken>());
        await planRepository.Received(2).AddPlanIndicatorAsync(Arg.Any<PlanIndicator>(), Arg.Any<CancellationToken>());
        await mediator.Received(2).Send(Arg.Any<CreatePlanDetailCommand>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Helper Methods

    private static CreatePlanCommand CreateValidCommand() => new()
    {
        PlanName = "測試計劃 2025",
        Year = 2025,
        CompanyId = 1,
        IndicatorId = "1,2",
        FactoryId = "F_01,F_02",
        IndustryId = "GRI",
        PlanTemplateIdList = [1, 2]
    };

    private static Scuser CreateMockUser() => new()
    {
        UserId = "TEST_USER",
        CurrentTenant = new CurrentTenant
        {
            UserId = "TEST_USER",
            TenantId = "TENANT_001"
        }
    };

    private static Plan CreateMockPlan() => new()
    {
        PlanId = 123,
        PlanName = "測試計劃 2025",
        CompanyId = 1,
        TenantId = "TENANT_001",
        Year = "2025",
        Show = true,
        CreatedUser = "TEST_USER",
        CreatedDate = DateTime.UtcNow
    };

    #endregion
}
```
