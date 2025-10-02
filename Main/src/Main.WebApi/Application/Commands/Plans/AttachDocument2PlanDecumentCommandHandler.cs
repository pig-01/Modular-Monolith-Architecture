using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Queries.Plans;
namespace Main.WebApi.Application.Commands.Plans;

/// <summary>
/// Attach Document to PlanDocument Command Handler
/// </summary>
/// <param name="logger"></param>
/// <param name="timeZoneService"></param>
/// <param name="userService"></param>
/// <param name="planRepository"></param>
public class AttachDocument2PlanDecumentCommandHandler(
    ILogger<AttachDocument2PlanDecumentCommandHandler> logger,
    IUserService<Scuser> userService,
    ITimeZoneService timeZoneService,
    IPlanRepository planRepository) : IRequestHandler<AttachDocument2PlanDecumentCommand, bool>
{
    [Authorize(Policy = "User")]
    public async Task<bool> Handle(AttachDocument2PlanDecumentCommand request, CancellationToken cancellationToken)
    {

        string createdUser = (await userService.Now(cancellationToken)).UserId;

        PlanDocument? newPlanDocument = new(request.PlanDetailId, request.StartDate, request.EndDate, request.planFactoryId, request.Year, request.Quarter, request.Month, createdUser, createdUser);

        newPlanDocument.Responsible = createdUser;

        PlanDocument? planDocument = await planRepository.AddPlanDocumentAsync(newPlanDocument, cancellationToken); ;

        planDocument.AttachDocument(request.DocumentId);

        logger.LogDebug("Attaching Document {DocumentId} to PlanDocument {PlanDocumentId}", request.DocumentId, planDocument.PlanDocumentId);

        await planRepository.UpdatePlanDocumentAsync(planDocument, timeZoneService.Now, cancellationToken);

        return true;
    }
}
