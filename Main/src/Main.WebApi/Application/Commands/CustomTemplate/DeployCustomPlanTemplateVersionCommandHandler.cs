using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 發布自訂計畫範本版本命令處理器
/// </summary>
public class DeployCustomPlanTemplateVersionCommandHandler(
    ILogger<DeployCustomPlanTemplateVersionCommandHandler> logger,
    IUserService<Scuser> userService,
    ITimeZoneService timeZoneService,
    ICustomRequestUnitRepository customRequestUnitRepository
) : IRequestHandler<DeployCustomPlanTemplateVersionCommand, bool>
{
    public async Task<bool> Handle(DeployCustomPlanTemplateVersionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Scuser currentUser = await userService.Now(cancellationToken);
            int updatedCount = await customRequestUnitRepository.DeployVersionAsync(request.RequestUnitId, request.VersionId, timeZoneService.Now, currentUser.UserId, cancellationToken);
            if (updatedCount > 0)
            {
                logger.LogInformation("User {User} deployed CustomPlanTemplateVersion {VersionId} for RequestUnit {RequestUnitId}.",
                    currentUser.UserId, request.VersionId, request.RequestUnitId);
                return true;
            }
            else
            {
                logger.LogWarning("User {User} attempted to deploy non-existing CustomPlanTemplateVersion {VersionId} for RequestUnit {RequestUnitId}.",
                    currentUser.UserId, request.VersionId, request.RequestUnitId);
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while user {User} was deploying CustomPlanTemplateVersion {VersionId} for RequestUnit {RequestUnitId}.",
                (await userService.Now(cancellationToken)).UserId, request.VersionId, request.RequestUnitId);
            return false;
        }
    }
}
