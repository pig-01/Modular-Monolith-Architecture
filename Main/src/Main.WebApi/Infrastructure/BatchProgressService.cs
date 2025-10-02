using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Base.Infrastructure.Interface.Progress;
using Main.Domain.Enums;
using Main.WebApi.Application.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Main.WebApi.Infrastructure;

public class BatchProgressService(
    ILogger<BatchProgressService> logger,
    IHubContext<BatchHub> hubContext) : IBatchProgressService
{
    public async Task ReportProgress(Guid batchId, int percentage, string? message = default)
    {
        await hubContext.Clients.Group($"batch_{batchId}").SendAsync("BatchProgress", new BatchProgress
        {
            BatchId = batchId,
            Percentage = (uint)percentage,
            Message = message,
            Status = BatchStatus.InProgress
        });

        logger.LogInformation("Reported progress for batch {BatchId}: {Percentage}%, Message: {Message}", $"batch_{batchId}", percentage, message);
    }

    public async Task ReportComplete(Guid batchId, string? message = default)
    {
        await hubContext.Clients.Group($"batch_{batchId}").SendAsync("BatchComplete", new BatchProgress
        {
            BatchId = batchId,
            Percentage = 100,
            Message = message,
            Status = BatchStatus.Completed
        });

        logger.LogInformation("Reported completion for batch {BatchId}, Message: {Message}", $"batch_{batchId}", message);
    }

    public async Task ReportError<TError>(Guid batchId, string? message = default, TError? err = default)
    {
        await hubContext.Clients.Group($"batch_{batchId}").SendAsync("BatchError", new BatchError<TError>
        {
            BatchId = batchId,
            Message = message,
            Error = err
        });

        logger.LogInformation("Reported error for batch {BatchId}, Message: {Message}, Error: {@Error}", $"batch_{batchId}", message, err);
    }

    public class BatchProgress
    {
        [JsonPropertyName("batchId")]
        public Guid BatchId { get; set; }

        [JsonPropertyName("percentage")]
        [Range(0, 100, ErrorMessage = "數值必須介於 0 到 100 之間")]
        public uint Percentage { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("status")]
        public required BatchStatus Status { get; set; }
    }

    public class BatchError<TError>
    {
        [JsonPropertyName("batchId")]
        public Guid BatchId { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("error")]
        public TError? Error { get; set; }
    }
}