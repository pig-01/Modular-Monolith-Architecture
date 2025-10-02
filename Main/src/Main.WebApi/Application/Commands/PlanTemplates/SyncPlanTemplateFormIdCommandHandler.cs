using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Commands.Bizform.Forms;
using Main.WebApi.Application.Models.Bizform.Forms;

namespace Main.WebApi.Application.Commands.PlanTemplates;

public class SyncPlanTemplateFormIdCommandHandler(
    IPlanTemplateRepository planTemplateRepository,
    IUserService<Scuser> userService,
    IMediator mediator
) : IRequestHandler<SyncPlanTemplateFormIdCommand, bool>
{
    public async Task<bool> Handle(SyncPlanTemplateFormIdCommand request, CancellationToken cancellationToken)
    {
        Scuser user = await userService.Now(cancellationToken);

        GetFormsCommand command = new()
        {
            Page = 1,
            PageSize = 9999
        };
        IEnumerable<Form> forms = await mediator.Send(command, cancellationToken) ?? throw new HandleException("Failed to retrieve forms");
        Dictionary<string, long> formIdMap = forms
            .Where(f => !string.IsNullOrWhiteSpace(f.DisplayName) && f.Id.HasValue)
            .ToDictionary(f => f.DisplayName, f => f.Id!.Value);

        int result = await planTemplateRepository.SyncPlanTemplateFormIdAsync(
            request.Version ?? string.Empty,
            user.CurrentTenant.TenantId,
            formIdMap,
            cancellationToken);

        return result > 0;
    }
}
