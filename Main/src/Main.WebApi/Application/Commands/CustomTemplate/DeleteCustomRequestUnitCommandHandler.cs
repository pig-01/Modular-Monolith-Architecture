using Main.Domain.AggregatesModel.CustomTemplateAggregate;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 刪除自訂要求單位命令處理器
/// </summary>
public class DeleteCustomRequestUnitCommandHandler(
    ILogger<DeleteCustomRequestUnitCommandHandler> logger,
    ICustomRequestUnitRepository repository
) : IRequestHandler<DeleteCustomRequestUnitCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomRequestUnitCommand request, CancellationToken cancellationToken)
    {
        try
        {
            int deletedCount = await repository.DeleteAsync(request.RequestUnitId, cancellationToken);
            if (deletedCount > 0)
            {
                logger.LogInformation("Deleted CustomRequestUnit {RequestUnitId}.", request.RequestUnitId);
                return true;
            }
            else
            {
                logger.LogWarning("Attempted to delete non-existing CustomRequestUnit {RequestUnitId}.", request.RequestUnitId);
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting CustomRequestUnit {RequestUnitId}.", request.RequestUnitId);
            return false;
        }
    }
}