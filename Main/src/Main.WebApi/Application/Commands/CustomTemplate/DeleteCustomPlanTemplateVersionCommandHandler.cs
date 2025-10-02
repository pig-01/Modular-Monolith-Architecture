using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 刪除自訂計畫範本版本命令處理器
/// </summary>
public class DeleteCustomPlanTemplateVersionCommandHandler(
    ILogger<DeleteCustomPlanTemplateVersionCommandHandler> logger,
    IUserService<Scuser> userService,
    ICustomRequestUnitRepository customRequestUnitRepository
) : IRequestHandler<DeleteCustomPlanTemplateVersionCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomPlanTemplateVersionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Scuser currentUser = await userService.Now(cancellationToken);
            int deletedCount = await customRequestUnitRepository.DeleteVersionAsync(request.RequestUnitId, request.VersionId, cancellationToken);
            if (deletedCount > 0)
            {
                logger.LogInformation("User {User} deleted CustomPlanTemplateVersion {VersionId} from RequestUnit {RequestUnitId}.",
                    currentUser.UserId, request.VersionId, request.RequestUnitId);
                return true;
            }
            else
            {
                logger.LogWarning("User {User} attempted to delete non-existing CustomPlanTemplateVersion {VersionId} from RequestUnit {RequestUnitId}.",
                    currentUser.UserId, request.VersionId, request.RequestUnitId);
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while user {User} was deleting CustomPlanTemplateVersion {VersionId} from RequestUnit {RequestUnitId}.",
                (await userService.Now(cancellationToken)).UserId, request.VersionId, request.RequestUnitId);
            return false;
        }
    }
}
