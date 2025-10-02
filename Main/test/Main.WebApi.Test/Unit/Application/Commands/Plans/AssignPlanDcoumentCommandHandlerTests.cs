using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Commands.Plans;
using Main.WebApi.Application.Queries.Plans;
using Main.WebApi.Application.Queries.Users;
using MediatR;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Main.WebApi.Test.Unit.Application.Commands.Plans;

/// <summary>
/// AssignPlanDcoumentCommandHandler 單元測試
/// 測試計畫文件指派功能的各種情境
/// </summary>
public class AssignPlanDcoumentCommandHandlerTests
{
    private readonly ITestOutputHelper output;
    private readonly ILogger<AssignPlanDcoumentCommandHandler> logger;
    private readonly IUserQuery subUserQuery;
    private readonly IPlanQuery subPlanQuery;
    private readonly IPlanRepository subPlanRepository;
    private readonly IUserService<Scuser> subUserService;
    private readonly AssignPlanDcoumentCommandHandler handler;

    public AssignPlanDcoumentCommandHandlerTests(ITestOutputHelper output)
    {
        this.output = output;
        logger = new TestLogger<AssignPlanDcoumentCommandHandler>(output);
        subUserQuery = Substitute.For<IUserQuery>();
        subPlanQuery = Substitute.For<IPlanQuery>();
        subPlanRepository = Substitute.For<IPlanRepository>();
        subUserService = Substitute.For<IUserService<Scuser>>();

        handler = new AssignPlanDcoumentCommandHandler(
            logger,
            subUserQuery,
            subPlanQuery,
            subPlanRepository,
            subUserService);
    }

    [Fact(DisplayName = "成功指派計畫文件給負責人")]
    public async Task Handle_ValidRequest_SuccessfullyAssignsPlanDocument()
    {
        // Arrange
        int planId = 1;
        string responsiblePersonId = "test@example.com";
        string currentUserId = "admin@example.com";
        int planDetailId = 100;

        AssignPlanDocumentCommand request = new()
        {
            PlanId = planId,
            ResponsiblePerson = responsiblePersonId,
            DataList =
            [
                new AssignPlanDetail
                {
                    PlanDetailId = planDetailId,
                    StartDate = "2024-01-01",
                    EndDate = "2024-12-31",
                    Year = 2024,
                    Quarter = null,
                    Month = null
                }
            ]
        };

        Plan plan = CreateTestPlan(planId, planDetailId);
        Scuser responsibleUser = CreateTestUser(responsiblePersonId);
        Scuser currentUser = CreateTestUser(currentUserId);

        subPlanQuery.GetByIdAsync(planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        subUserQuery.GetByIdAsync(responsiblePersonId, Arg.Any<CancellationToken>())
            .Returns(responsibleUser);
        subUserService.Now(Arg.Any<CancellationToken>())
            .Returns(currentUser);

        // Act
        MediatR.Unit result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(MediatR.Unit.Value, result);

        // Verify repository method was called
        await subPlanRepository.Received(1)
            .AssignPlanDocumentAsync(
                planId,
                responsiblePersonId,
                currentUserId,
                Arg.Any<IEnumerable<PlanDocument>>(),
                Arg.Any<CancellationToken>());

        // Verify plan assign method was called
        Assert.True(plan.DomainEvents.Any());
    }

    [Fact(DisplayName = "計畫不存在時拋出 NotFoundException")]
    public async Task Handle_PlanNotFound_ThrowsNotFoundException()
    {
        // Arrange
        AssignPlanDocumentCommand request = new()
        {
            PlanId = 999,
            ResponsiblePerson = "test@example.com",
            DataList =
            [
                new AssignPlanDetail
                {
                    PlanDetailId = 100,
                    StartDate = "2024-01-01",
                    EndDate = "2024-12-31",
                    Year = 2024,
                    Quarter = null,
                    Month = null
                }
            ]
        };

        subPlanQuery.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Plan?)null);

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(request, CancellationToken.None));

        Assert.Equal("Plan is not found", exception.Message);
    }

    [Fact(DisplayName = "負責人不存在時拋出 NotFoundException")]
    public async Task Handle_ResponsiblePersonNotFound_ThrowsNotFoundException()
    {
        // Arrange
        int planId = 1;
        int planDetailId = 100;
        AssignPlanDocumentCommand request = new()
        {
            PlanId = planId,
            ResponsiblePerson = "nonexistent@example.com",
            DataList =
            [
                new AssignPlanDetail
                {
                    PlanDetailId = planDetailId,
                    StartDate = "2024-01-01",
                    EndDate = "2024-12-31",
                    Year = 2024,
                    Quarter = null,
                    Month = null
                }
            ]
        };

        Plan plan = CreateTestPlan(planId, planDetailId);

        subPlanQuery.GetByIdAsync(planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        subUserQuery.GetByIdAsync("nonexistent@example.com", Arg.Any<CancellationToken>())
            .Returns((Scuser?)null);

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(request, CancellationToken.None));

        Assert.Equal("Responsible person is not found", exception.Message);
    }

    [Fact(DisplayName = "計畫明細不存在於計畫中時拋出 HandleException")]
    public async Task Handle_PlanDetailNotInPlan_ThrowsHandleException()
    {
        // Arrange
        int planId = 1;
        int validPlanDetailId = 100;
        int invalidPlanDetailId = 999;
        string responsiblePersonId = "test@example.com";

        AssignPlanDocumentCommand request = new()
        {
            PlanId = planId,
            ResponsiblePerson = responsiblePersonId,
            DataList =
            [
                new AssignPlanDetail
                {
                    PlanDetailId = invalidPlanDetailId, // 不存在於計畫中的明細ID
                    StartDate = "2024-01-01",
                    EndDate = "2024-12-31",
                    Year = 2024,
                    Quarter = null,
                    Month = null
                }
            ]
        };

        Plan plan = CreateTestPlan(planId, validPlanDetailId); // 只包含validPlanDetailId
        Scuser responsibleUser = CreateTestUser(responsiblePersonId);

        subPlanQuery.GetByIdAsync(planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        subUserQuery.GetByIdAsync(responsiblePersonId, Arg.Any<CancellationToken>())
            .Returns(responsibleUser);

        // Act & Assert
        HandleException exception = await Assert.ThrowsAsync<HandleException>(
            () => handler.Handle(request, CancellationToken.None));

        Assert.Equal("Some details are not included in the plan", exception.Message);
    }

    [Theory(DisplayName = "無效的日期格式拋出 ParameterException")]
    [InlineData("invalid-date", "2024-12-31", "StartDate Parse Failed")]
    [InlineData("2024-01-01", "invalid-date", "EndDate Parse Failed")]
    [InlineData("", "2024-12-31", "StartDate Parse Failed")]
    [InlineData("2024-01-01", "", "EndDate Parse Failed")]
    public async Task Handle_InvalidDateFormat_ThrowsParameterException(
        string startDate, string endDate, string expectedMessage)
    {
        // Arrange
        int planId = 1;
        int planDetailId = 100;
        string responsiblePersonId = "test@example.com";
        string currentUserId = "admin@example.com";

        AssignPlanDocumentCommand request = new()
        {
            PlanId = planId,
            ResponsiblePerson = responsiblePersonId,
            DataList =
            [
                new AssignPlanDetail
                {
                    PlanDetailId = planDetailId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Year = 2024,
                    Quarter = null,
                    Month = null
                }
            ]
        };

        Plan plan = CreateTestPlan(planId, planDetailId);
        Scuser responsibleUser = CreateTestUser(responsiblePersonId);
        Scuser currentUser = CreateTestUser(currentUserId);

        subPlanQuery.GetByIdAsync(planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        subUserQuery.GetByIdAsync(responsiblePersonId, Arg.Any<CancellationToken>())
            .Returns(responsibleUser);
        subUserService.Now(Arg.Any<CancellationToken>())
            .Returns(currentUser);

        // Act & Assert
        ParameterException exception = await Assert.ThrowsAsync<ParameterException>(
            () => handler.Handle(request, CancellationToken.None));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact(DisplayName = "多個計畫明細成功指派")]
    public async Task Handle_MultiplePlanDetails_SuccessfullyAssignsAllDocuments()
    {
        // Arrange
        int planId = 1;
        string responsiblePersonId = "test@example.com";
        string currentUserId = "admin@example.com";
        int[] planDetailIds = [100, 101, 102];

        AssignPlanDocumentCommand request = new()
        {
            PlanId = planId,
            ResponsiblePerson = responsiblePersonId,
            DataList = planDetailIds.Select(id => new AssignPlanDetail
            {
                PlanDetailId = id,
                StartDate = "2024-01-01",
                EndDate = "2024-12-31",
                Year = 2024,
                Quarter = null,
                Month = null
            }).ToList()
        };

        Plan plan = CreateTestPlanWithMultipleDetails(planId, planDetailIds);
        Scuser responsibleUser = CreateTestUser(responsiblePersonId);
        Scuser currentUser = CreateTestUser(currentUserId);

        subPlanQuery.GetByIdAsync(planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        subUserQuery.GetByIdAsync(responsiblePersonId, Arg.Any<CancellationToken>())
            .Returns(responsibleUser);
        subUserService.Now(Arg.Any<CancellationToken>())
            .Returns(currentUser);

        // Act
        MediatR.Unit result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(MediatR.Unit.Value, result);

        // Verify repository was called with correct number of documents
        await subPlanRepository.Received(1)
            .AssignPlanDocumentAsync(
                planId,
                responsiblePersonId,
                currentUserId,
                Arg.Is<IEnumerable<PlanDocument>>(docs => docs.Count() == 3),
                Arg.Any<CancellationToken>());
    }

    #region Helper Methods

    /// <summary>
    /// 建立測試用的計畫實體
    /// </summary>
    private static Plan CreateTestPlan(int planId, int planDetailId)
    {
        Plan plan = new()
        {
            PlanId = planId,
            PlanName = "Test Plan",
            CompanyId = 1,
            TenantId = "TEST",
            Year = "2024",
            PlanDetails =
            [
                new PlanDetail
                {
                    PlanDetailId = planDetailId,
                    PlanDetailName = "Test Detail",
                    PlanId = planId,
                    PlanTemplateId = 1,
                    FormId = 1,
                    GroupId = "TEST_GROUP",
                    CycleType = "M",
                    CreatedUser = "TEST_USER",
                    CreatedDate = DateTime.UtcNow,
                    ModifiedUser = "TEST_USER",
                    ModifiedDate = DateTime.UtcNow,
                    PlanDocuments = new HashSet<PlanDocument>()
                }
            ]
        };

        // 設定 PlanDetail 的 Plan 參考
        foreach (var detail in plan.PlanDetails)
        {
            detail.Plan = plan;
        }

        return plan;
    }

    /// <summary>
    /// 建立包含多個明細的測試計畫
    /// </summary>
    private static Plan CreateTestPlanWithMultipleDetails(int planId, int[] planDetailIds)
    {
        Plan plan = new()
        {
            PlanId = planId,
            PlanName = "Test Plan",
            CompanyId = 1,
            TenantId = "TEST",
            Year = "2024",
            PlanDetails = planDetailIds.Select(id => new PlanDetail
            {
                PlanDetailId = id,
                PlanDetailName = $"Test Detail {id}",
                PlanId = planId,
                PlanTemplateId = 1,
                FormId = 1,
                GroupId = "TEST_GROUP",
                CycleType = "M",
                CreatedUser = "TEST_USER",
                CreatedDate = DateTime.UtcNow,
                ModifiedUser = "TEST_USER",
                ModifiedDate = DateTime.UtcNow,
                PlanDocuments = new HashSet<PlanDocument>()
            }).ToList()
        };

        // 設定 PlanDetail 的 Plan 參考
        foreach (var detail in plan.PlanDetails)
        {
            detail.Plan = plan;
        }

        return plan;
    }

    /// <summary>
    /// 建立測試用的使用者實體
    /// </summary>
    private static Scuser CreateTestUser(string userId) => new()
    {
        UserId = userId,
        UserName = "Test User",
        PasswordHash = "test_hash",
        UserType = "1"
    };

    #endregion
}
