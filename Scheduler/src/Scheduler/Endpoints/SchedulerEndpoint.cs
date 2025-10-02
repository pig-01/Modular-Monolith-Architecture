using System.Reflection;
using Scheduler.Application.Jobs.Background;
using Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using Serilog;

namespace Scheduler.Endpoints;

public static class SchedulerEndpoint
{
    public const string GroupName = "Scheduler";

    /// <summary>
    /// Maps the scheduler endpoints for version 1.0 of the API.
    /// </summary>
    /// <param name="app"></param>
    public static void MapSchedulerV1Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapSchedulerEndpoint();
        app.MapSchedulerGroupEndpoint();
    }

    /// <summary>
    /// Maps the scheduler endpoints for the application.
    /// </summary>
    /// <param name="app"></param>
    private static void MapSchedulerEndpoint(this IEndpointRouteBuilder app)
    {
        // Map Quartz scheduler endpoints
        RouteGroupBuilder schedulerGroup = app.MapGroup("/scheduler")
            .WithTags("Scheduler")
            .WithDescription("Endpoints for managing Quartz jobs and triggers");

        _ = schedulerGroup.MapGet("/jobs", async (ISchedulerFactory schedulerFactory, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IReadOnlyCollection<JobKey> jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), token);
            return Results.Ok(jobKeys);
        }).WithName("GetAllJobs")
          .WithSummary("Get all scheduled jobs")
          .WithDescription("Retrieves all job keys currently scheduled in the Quartz scheduler.");

        _ = schedulerGroup.MapPost("/jobs", async (ISchedulerFactory schedulerFactory, [FromBody] RegisterJobRequest request, CancellationToken token) =>
        {
            // 1. 根據 jobName 找到 Type
            Type? jobType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(SafeGetTypes)
                .FirstOrDefault(t => t.Name == request.JobName && typeof(IJob).IsAssignableFrom(t));

            if (jobType is null)
            {
                return Results.NotFound($"Job type not found for job name: {request.JobName}");
            }

            // 2. JobDetail
            IJobDetail jobDetail = JobBuilder.Create(jobType)
                .WithIdentity($"{request.JobName}_{Guid.NewGuid()}")
                .Build();

            // 3. Trigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithCronSchedule(request.CronExpression)
                .Build();

            IScheduler scheduler = await schedulerFactory.GetScheduler(token);

            // 4. 註冊進 scheduler
            _ = await scheduler.ScheduleJob(jobDetail, trigger, token);

            Log.Information($"Job '{request.JobName}' has been scheduled.");
            return Results.Ok($"Job '{request.JobName}' has been scheduled.");
        }).WithName("RegisterJob")
          .WithSummary("Register a new job")
          .WithDescription("Registers a new job with the specified job name, cron expression, and optional job arguments.");


        _ = schedulerGroup.MapGet("/jobs/{jobId}/triggers", async (ILogger<Program> logger, ISchedulerFactory schedulerFactory, string jobId, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IReadOnlyCollection<ITrigger> triggers = await scheduler.GetTriggersOfJob(new JobKey(jobId), token);
            if (triggers is not null)
            {
                logger.LogInformation("Retrieved job detail for job ID: {JobId}", jobId);
                logger.LogInformation("Job triggers: {Triggers}", triggers);
                return Results.Ok(triggers);
            }
            return Results.NotFound();
        }).WithName("GetJobTriggers")
          .WithSummary("Get triggers for a specific job by ID")
          .WithDescription("Retrieves the triggers associated with a specific job identified by its ID.");

        _ = schedulerGroup.MapDelete("/jobs/{jobId}", async (ISchedulerFactory schedulerFactory, string jobId, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IJobDetail? jobDetail = await scheduler.GetJobDetail(new JobKey(jobId), token);
            if (jobDetail is not null)
            {
                _ = await scheduler.DeleteJob(new JobKey(jobId), token);
                Log.Information($"Job '{jobId}' has been deleted.");
                return Results.Ok($"Job '{jobId}' has been deleted.");
            }
            return Results.NotFound();
        }).WithName("DeleteJob")
          .WithSummary("Delete a specific job by ID")
          .WithDescription("Deletes a specific job identified by its ID.");

        _ = schedulerGroup.MapPost("/jobs/{jobId}/pause", async (ISchedulerFactory schedulerFactory, string jobId, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IJobDetail? jobDetail = await scheduler.GetJobDetail(new JobKey(jobId), token);
            if (jobDetail is not null)
            {
                await scheduler.PauseJob(new JobKey(jobId), token);
                Log.Information($"Job '{jobId}' has been paused.");
                return Results.Ok($"Job '{jobId}' has been paused.");
            }
            return Results.NotFound();
        }).WithName("PauseJob")
          .WithSummary("Pause a specific job by ID")
          .WithDescription("Pauses a specific job identified by its ID.");

        _ = schedulerGroup.MapPost("/jobs/{jobId}/resume", async (ISchedulerFactory schedulerFactory, string jobId, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IJobDetail? jobDetail = await scheduler.GetJobDetail(new JobKey(jobId), token);
            if (jobDetail is not null)
            {
                await scheduler.ResumeJob(new JobKey(jobId), token);
                Log.Information($"Job '{jobId}' has been resumed.");
                return Results.Ok($"Job '{jobId}' has been resumed.");
            }
            return Results.NotFound();
        }).WithName("ResumeJob")
          .WithSummary("Resume a specific job by ID")
          .WithDescription("Resumes a specific job identified by its ID.");
    }

    /// <summary>
    /// Maps the scheduler group endpoints for the application.
    /// </summary>
    /// <param name="app"></param>
    private static void MapSchedulerGroupEndpoint(this IEndpointRouteBuilder app)
    {
        // Map Quartz scheduler with group endpoints
        RouteGroupBuilder schedulerGGroup = app.MapGroup("/scheduler/{group}")
            .WithTags("SchedulerGroup")
            .WithDescription("Endpoints for managing Quartz jobs and triggers");

        _ = schedulerGGroup.MapPost("/jobs/notified", async (ISchedulerFactory schedulerFactory, string group, string message, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IJobDetail jobDetail = JobBuilder.Create<MailJob>()
                .WithIdentity("MailJob", group)
                .UsingJobData("Message", message)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("MailJobTrigger", group)
                .StartNow()
                .WithCronSchedule("0/5 * * * * ?") // Every 5 seconds
                .Build();

            _ = await scheduler.ScheduleJob(jobDetail, trigger, token);
            Log.Information($"NotifiedJob has been scheduled with message: {message}");
            return Results.Ok($"NotifiedJob has been scheduled with message: {message}");
        }).WithSummary("Schedule a NotifiedJob with a message")
          .WithDescription("Schedules a NotifiedJob that logs a message every 5 seconds.");

        _ = schedulerGGroup.MapGet("/jobs/{jobId}/triggers", async (ILogger<Program> logger, ISchedulerFactory schedulerFactory, string group, string jobId, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IReadOnlyCollection<ITrigger> triggers = await scheduler.GetTriggersOfJob(new JobKey(jobId, group), token);
            if (triggers is not null)
            {
                logger.LogInformation("Retrieved job detail for job ID: {JobId}", jobId);
                logger.LogInformation("Job triggers: {Triggers}", triggers);
                return Results.Ok(triggers);
            }
            return Results.NotFound();
        }).WithSummary("Get a specific job by ID")
          .WithDescription("Retrieves the job detail for a specific job identified by its ID.");

        _ = schedulerGGroup.MapDelete("/jobs/{jobId}", async (ISchedulerFactory schedulerFactory, string group, string jobId, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IJobDetail? jobDetail = await scheduler.GetJobDetail(new JobKey(jobId, group), token);
            if (jobDetail is not null)
            {
                _ = await scheduler.DeleteJob(new JobKey(jobId, group), token);
                Log.Information($"Job '{jobId}' has been deleted.");
                return Results.Ok($"Job '{jobId}' has been deleted.");
            }
            return Results.NotFound();
        }).WithSummary("Delete a specific job by ID")
          .WithDescription("Deletes a specific job identified by its ID.");

        _ = schedulerGGroup.MapPost("/jobs/{jobId}/pause", async (ISchedulerFactory schedulerFactory, string group, string jobId, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IJobDetail? jobDetail = await scheduler.GetJobDetail(new JobKey(jobId, group), token);
            if (jobDetail is not null)
            {
                await scheduler.PauseJob(new JobKey(jobId, group), token);
                Log.Information($"Job '{jobId}' has been paused.");
                return Results.Ok($"Job '{jobId}' has been paused.");
            }
            return Results.NotFound();
        }).WithSummary("Pause a specific job by ID")
          .WithDescription("Pauses a specific job identified by its ID.");

        _ = schedulerGGroup.MapPost("/jobs/{jobId}/resume", async (ISchedulerFactory schedulerFactory, string group, string jobId, CancellationToken token) =>
        {
            IScheduler scheduler = await schedulerFactory.GetScheduler(token);
            IJobDetail? jobDetail = await scheduler.GetJobDetail(new JobKey(jobId, group), token);
            if (jobDetail is not null)
            {
                await scheduler.ResumeJob(new JobKey(jobId, group), token);
                Log.Information($"Job '{jobId}' has been resumed.");
                return Results.Ok($"Job '{jobId}' has been resumed.");
            }
            return Results.NotFound();
        }).WithSummary("Resume a specific job by ID")
          .WithDescription("Resumes a specific job identified by its ID.");
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
        catch
        {
            return [];
        }
    }
}
