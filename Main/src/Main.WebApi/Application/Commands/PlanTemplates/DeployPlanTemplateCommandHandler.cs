using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Commands.PlanTemplates;

public class DeployPlanTemplateCommandHandler(
    ILogger<DeployPlanTemplateCommandHandler> logger,
    IPlanTemplateRepository planTemplateRepository,
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService) : IRequestHandler<DeployPlanTemplateCommand, bool>
{
    [Authorize(Policy = "User")]
    public async Task<bool> Handle(DeployPlanTemplateCommand request, CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);
        DateTime now = timeZoneService.Now;

        logger.LogInformation("開始部署版本 {Version} 的計畫樣板", request.Version);

        // 使用 Repository 方法進行批次更新
        int updatedCount = await planTemplateRepository.DeployPlanTemplatesByVersionAsync(
            request.Version,
            now,
            currentUser.UserId,
            cancellationToken);

        if (updatedCount == 0)
        {
            logger.LogWarning("版本 {Version} 沒有找到任何計畫樣板", request.Version);
            throw new HandleException($"版本 {request.Version} 沒有找到任何計畫樣板");
        }

        logger.LogInformation("成功部署版本 {Version} 的 {Count} 個計畫樣板", request.Version, updatedCount);

        return true;
    }
}
