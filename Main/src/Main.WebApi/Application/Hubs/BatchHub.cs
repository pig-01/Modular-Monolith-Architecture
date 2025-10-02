using Microsoft.AspNetCore.SignalR;

namespace Main.WebApi.Application.Hubs;

public class BatchHub(ILogger<BatchHub> logger) : Hub
{
    public async Task JoinBatchGroup(Guid taskId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"batch_{taskId}");
        logger.LogInformation("Connection {ConnectionId} joined group {TaskId}", Context.ConnectionId, $"batch_{taskId}");
    }

    public async Task LeaveBatchGroup(Guid taskId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"batch_{taskId}");
        logger.LogInformation("Connection {ConnectionId} left group {TaskId}", Context.ConnectionId, $"batch_{taskId}");
    }
}