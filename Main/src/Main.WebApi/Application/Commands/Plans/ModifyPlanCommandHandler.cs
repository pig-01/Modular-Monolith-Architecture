using System.Globalization;
using Base.Domain.Exceptions;
using Base.Infrastructure.Toolkits.Extensions;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.WebApi.Application.Queries.Plans;

namespace Main.WebApi.Application.Commands.Plans;

public class ModifyPlanCommandHandler(
    ILogger<ModifyPlanCommandHandler> logger,
    IHttpContextAccessor httpContextAccessor,
    IPlanRepository planRepository,
    IPlanQuery planQuery) : IRequestHandler<ModifyPlanCommand, bool>
{

    [Authorize(Policy = "User")]
    public async Task<bool> Handle(ModifyPlanCommand request, CancellationToken cancellationToken)
    {
        Plan? plan = await planQuery.GetByIdAsync(request.PlanId, cancellationToken) ?? throw new HandleException(nameof(Plan));
        plan.PlanName = request.PlanName;
        plan.Year = request.Year!.Value.ToString(CultureInfo.InvariantCulture);
        plan.ModifiedUser = httpContextAccessor.HttpContext?.User.Claims.GetNameId() ?? "System";
        logger.LogInformation("Update Plan: {@Plan}", plan);
        await planRepository.UpdateAsync(plan, cancellationToken);
        return true;
    }
}
