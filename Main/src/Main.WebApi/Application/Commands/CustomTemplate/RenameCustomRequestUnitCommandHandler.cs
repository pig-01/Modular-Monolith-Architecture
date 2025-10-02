using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 重新命名要求單位或版本命令處理器
/// </summary>
public class RenameCustomRequestUnitCommandHandler(
    ILogger<RenameCustomRequestUnitCommandHandler> logger,
    IUserService<Scuser> userService,
    ICustomRequestUnitRepository repository) : IRequestHandler<RenameCustomRequestUnitCommand, CustomRequestUnit>
{
    public async Task<CustomRequestUnit> Handle(RenameCustomRequestUnitCommand request, CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);

        if (!request.VersionId.HasValue)
        {
            // Rename existing CustomRequestUnit
            CustomRequestUnit entity = await repository.RenameAsync(request.RequestUnitId!.Value, request.RequestUnitName!, currentUser.UserId, cancellationToken);
            return entity;
        }
        else
        {
            // Rename existing CustomPlanTemplateVersion
            CustomPlanTemplateVersion entity = await repository.RenameVersionAsync(request.RequestUnitId!.Value, request.VersionId!.Value, request.Version!, currentUser.UserId, cancellationToken);
            return entity.Unit;
        }
    }
}