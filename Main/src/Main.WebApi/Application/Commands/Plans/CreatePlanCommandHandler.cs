using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Extensions;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Queries.CustomTemplate;

namespace Main.WebApi.Application.Commands.Plans;

public class CreatePlanCommandHandler(
    ILogger<CreatePlanCommandHandler> logger,
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService,
    IPlanRepository planRepository,
    ICustomRequestUnitQuery customRequestUnitQuery,
    IMediator mediator) : IRequestHandler<CreatePlanCommand, int>
{
    [Authorize(Policy = "User")]
    public async Task<int> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        Scuser scuser = await userService.Now(cancellationToken);

        // Validate request
        if (request.Year is null) throw new HandleException("Year is required");
        if (scuser.CurrentTenant?.TenantId is null) throw new HandleException("Tenant is required");

        IEnumerable<CustomRequestUnit> requestUnits = await customRequestUnitQuery.ListAsync(scuser.CurrentTenant.TenantId, cancellationToken);
        Dictionary<long, CustomRequestUnit> validCustomRequestUnits = requestUnits.ToDictionary(x => x.UnitId);

        // Add Plan
        Plan plan = Plan.Create(
            request.PlanName, scuser.CurrentTenant.TenantId!, request.CompanyId, request.Year!.Value,
            request.PlanTemplateVersion, timeZoneService.Now, scuser.UserId);

        // Add Plan Factory
        plan.PlanFactories = [.. request.FactoryIdList
            .Where(factoryId => !string.IsNullOrWhiteSpace(factoryId))
            .Select(factoryId => PlanFactory.Create(factoryId, scuser.CurrentTenant.TenantId, scuser.UserId, scuser.UserId))];

        // Add Plan Industry
        plan.PlanIndustries = [.. request.IndustryIdList
            .Where(industryId => !string.IsNullOrWhiteSpace(industryId))
            .Select(industryId => new PlanIndustry
            {
                IndustryId = industryId,
                CreatedUser = scuser.UserId,
                ModifiedUser = scuser.UserId
            })];

        // Add Plan Indicator
        plan.PlanIndicators = [.. request.IndicatorIdList
            .Where(indicatorId => !string.IsNullOrWhiteSpace(indicatorId))
            .Select(indicatorId => new PlanIndicator
            {
                IndicatorId = indicatorId,
                IndicatorType = "standard",
                CreatedUser = scuser.UserId,
                ModifiedUser = scuser.UserId
            })];

        // Add Plan Custom Indicator
        plan.PlanIndicators =
        [
            .. plan.PlanIndicators,
            .. request.CustomIndicatorIdList
                .Select(customIndicatorId => new PlanIndicator
                {
                    RequestUnitId = customIndicatorId,
                    VersionId = validCustomRequestUnits[customIndicatorId].LastVersion?.VersionId,
                    IndicatorType = "custom",
                    CreatedUser = scuser.UserId,
                    ModifiedUser = scuser.UserId
                }),
        ];

        logger.LogDebug("Create Plan {PlanId} Plan: {@Plan}", plan.PlanId, plan);
        _ = await planRepository.AddAsync(plan, cancellationToken);

        int rowNumber = 1;
        // Add PlanDetail By PlanTemplate
        foreach (int planTemplateId in request.PlanTemplateIdList)
        {
            CreatePlanDetailCommand insertPlanDetailCommand = new()
            {
                Year = request.Year,
                PlanTemplateId = planTemplateId,
                PlanId = plan.PlanId,
                RowNumber = rowNumber++

            };
            await mediator.Send(insertPlanDetailCommand, cancellationToken);
        }

        // Add PlanDetail By CustomPlanTemplate
        foreach (int customPlanTemplateId in request.CustomPlanTemplateIdList)
        {
            CreatePlanDetailCommand insertPlanDetailByCustomPlanTemplateCommand = new()
            {
                Year = request.Year,
                CustomPlanTemplateId = customPlanTemplateId,
                PlanId = plan.PlanId,
                RowNumber = rowNumber++
            };
            await mediator.Send(insertPlanDetailByCustomPlanTemplateCommand, cancellationToken);
        }


        return plan.PlanId;
    }
}
