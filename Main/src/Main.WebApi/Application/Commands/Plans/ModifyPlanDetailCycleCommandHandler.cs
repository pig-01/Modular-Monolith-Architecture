using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.WebApi.Application.Queries.Plans;

namespace Main.WebApi.Application.Commands.Plans;

/// <summary>
/// Modify PlanDetail Cycle Command Handler
/// </summary>
/// <remarks>
/// First, it modifies the cycle of the specified PlanDetail.
/// Second, it archives the PlanDocument related to the modified PlanDetail.
/// Third, if IsApplyAll is true, it modifies the cycle of all PlanDetails with the same CycleType.
/// Finally, it modifies the StartDate and EndDate of the PlanDocuments related to the modified PlanDetails.
/// </remarks>
/// <param name="logger"></param>
/// <param name="mediator"></param>
/// <param name="planDetailQuery"></param>
/// <param name="planDocumentQuery"></param>
/// <param name="planRepository"></param>
public class ModifyPlanDetailCycleCommandHandler(
    ILogger<ModifyPlanDetailCycleCommandHandler> logger,
    IMediator mediator,
    ITimeZoneService timeZoneService,
    IPlanDetailQuery planDetailQuery,
    IPlanDocumentQuery planDocumentQuery,
    IPlanRepository planRepository) : IRequestHandler<ModifyPlanDetailCycleCommand, bool>
{

    [Authorize(Policy = "User")]
    public async Task<bool> Handle(ModifyPlanDetailCycleCommand request, CancellationToken cancellationToken)
    {
        // Modify PlanDetail Cycle
        PlanDetail? planDetail = await planDetailQuery.GetByIdAsync(request.PlanDetailId, cancellationToken);

        if (planDetail == null)
        {
            logger.LogError("PlanDetail with ID {PlanDetailId} not found.", request.PlanDetailId);
            throw new NotFoundException($"PlanDetail with ID {request.PlanDetailId} not found.");
        }

        // Modify PlanDetail Cycle
        planDetail.ChangeCycleType(
            request.CycleType,
            request.CycleMonth,
            request.CycleDay,
            request.CycleMonthLast,
            request.EndDate);

        // Update PlanDetail
        await planRepository.UpdatePlanDetailAsync(planDetail, timeZoneService.Now, cancellationToken);

        // Archive PlanDocument which is related to PlanDetail
        IEnumerable<PlanDocument> planDocuments = await planDocumentQuery.ListByDetailIdAsync(request.PlanDetailId, cancellationToken);
        foreach (PlanDocument document in planDocuments)
        {
            ArchivePlanDocumentCommand command = new(document.PlanDocumentId);
            await mediator.Send(command, cancellationToken);
        }

        // When IsApplyAll is false then return
        if (!request.IsApplyAll) return true;

        // Modify PlanDetail which are in the same CycleType and Modify PlanDocument StartDate and EndDate which refer to modified PlanDetail
        IEnumerable<PlanDetail> planDetails = await planDetailQuery.ListByPlanIdAsync(planDetail.PlanId, cancellationToken);
        foreach (PlanDetail detail in planDetails)
        {
            // Skip the PlanDetail which is not in the same CycleType or is the same PlanDetail
            if (detail.PlanDetailId == request.PlanDetailId || detail.CycleType != request.CycleType) continue;

            detail.ChangeCycleType(
                request.CycleType,
                request.CycleMonth,
                request.CycleDay,
                request.CycleMonthLast,
                request.EndDate);

            await planRepository.UpdatePlanDetailAsync(detail, timeZoneService.Now, cancellationToken);

            // Modify PlanDocument StartDate and EndDate which refer to modified PlanDetail
            foreach (PlanDocument planDocument in detail.PlanDocuments)
            {
                // Get PlanDocumentCycle which is related to PlanDocument
                PlanDocumentCycle? planDocumentCycle =
                    GetPlanDocumentCycle(request.PlanDocumentCycleArray, detail.CycleType, planDocument.Year, planDocument.Quarter, planDocument.Month);

                // When PlanDocumentCycle is null then continue
                if (planDocumentCycle == null) continue;

                planDocument.ChangeCycleType(planDocumentCycle.StartDate, planDocumentCycle.EndDate);
                await planRepository.UpdatePlanDocumentAsync(planDocument, timeZoneService.Now, cancellationToken);
            }
        }

        return true;
    }

    private static PlanDocumentCycle? GetPlanDocumentCycle(List<PlanDocumentCycle> planDocumentCycleArray, string cycleType, int? year, int? quarter, int? month) => cycleType switch
    {
        "year" => planDocumentCycleArray.FirstOrDefault(x => x.Year == year),
        "quarter" => planDocumentCycleArray.FirstOrDefault(x => x.Quarter == quarter),
        "month" => planDocumentCycleArray.FirstOrDefault(x => x.Month == month),
        _ => throw new ArgumentException($"Invalid cycle type: {cycleType}"),
    };
}
