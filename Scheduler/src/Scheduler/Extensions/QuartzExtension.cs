using Quartz;
using Quartz.AspNetCore;
using Base.Infrastructure.Extension;

namespace Scheduler.Extensions;

public static class QuartzExtension
{
    /// <summary>
    /// Configure Quartz services and options.
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // base configuration from appsettings.json
        _ = builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));

        // if you are using persistent job store, you might want to alter some options
        _ = builder.Services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
    }

    /// <summary>
    /// Adds Quartz.NET services to the DI container.
    /// </summary>
    /// <param name="services"></param>
    public static void AddQuartzScheduler(this WebApplicationBuilder builder)
    {
        // Configure Quartz services
        builder.Services.AddQuartz(configure =>
        {
            // handy when part of cluster or you want to otherwise identify multiple schedulers
            configure.SchedulerId = "Scheduler-Core";

            configure.UseSimpleTypeLoader();
            configure.UseDefaultThreadPool(configure => configure.MaxConcurrency = 10);

            // Configure persistent store
            configure.UsePersistentStore(store =>
            {
                store.UseProperties = false;
                store.RetryInterval = TimeSpan.FromSeconds(30);
                store.UseSqlServer(mssql =>
                {
                    mssql.ConnectionString = builder.Configuration.GetConnectionString("QuartzConnectionString")!.DecryptString() ?? throw new InvalidOperationException("QuartzConnectionString is not configured.");
                    mssql.TablePrefix = "QRTZ_";
                });
                store.UseSystemTextJsonSerializer();
                store.UseClustering(c =>
                {
                    c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                    c.CheckinInterval = TimeSpan.FromSeconds(10);
                });
            });

            // Configure default background job store
            configure.UseDefaultBackgroundJobStore();
        });

        // ASP.NET Core hosting
        _ = builder.Services.AddQuartzServer(options => options.WaitForJobsToComplete = true);
    }
}