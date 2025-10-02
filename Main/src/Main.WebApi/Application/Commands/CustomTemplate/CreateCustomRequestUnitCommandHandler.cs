using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Commands.CustomTemplate;

public class CreateCustomRequestUnitCommandHandler(ILogger<CreateCustomRequestUnitCommandHandler> logger,
    IUserService<Scuser> userService,
    ICustomRequestUnitRepository repository) : IRequestHandler<CreateCustomRequestUnitCommand, CustomRequestUnit>
{
    public async Task<CustomRequestUnit> Handle(CreateCustomRequestUnitCommand request, CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);

        return await repository.AddAsync(
            request.RequestUnitId,
            request.RequestUnitName,
            request.Version,
            currentUser.CurrentTenant.TenantId,
            currentUser.UserId,
            cancellationToken);
    }
}
