using Dapper;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Extension;
using Main.Infrastructure.Demo.Context;
using Main.Infrastructure.Services;
using Main.Repository.Handlers;

namespace Main.WebApi.Extensions;

/// <summary>
/// 擴展資料來源
/// </summary>
public static class DataSourceExtension
{
    /// <summary>
    /// 擴展 DbContext
    /// </summary>
    /// <param name="builder"></param>
    public static void AddDbContext(this WebApplicationBuilder builder)
    {
        // 加入 EF Core DbContext
        builder.Services.AddDbContext<DbContext, DemoContext>(options =>
        {
#if RELEASE
            // 使用 SQL Server
            options.UseSqlServer(builder.Configuration.GetConnectionString("BdbuDemoPreProdAzureConnection")!.DecryptString(),
                    providerOptions => providerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .EnableSensitiveDataLogging(false);
#else
            // 使用 SQLite (開發環境使用 SQLite，生產環境使用 SQL Server)
            options.UseSqlServer(builder.Configuration.GetConnectionString("BdbuDemoDev2022Connection")!.DecryptString(),
                    providerOptions => providerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .EnableSensitiveDataLogging(false);
#endif

            // 設定 EF Core 日誌
            options.EnableSensitiveDataLogging(false)
                .EnableDetailedErrors(false);

        });

        // 註冊 DemoContext UnitOfWork 元件
        // 先不考慮跨 DB 的情況和多 DbContext 的情況
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    /// <summary>
    /// 擴展 Dapper SqlMapper 處理器 in DI
    /// </summary>
    /// <param name="services"></param>
    public static void AddSqlMapper(this IServiceCollection services)
    {
        // 註冊 Dapper SqlMapper 處理器 in DI
        services.AddScoped<DateTimeHandler>();
        services.AddScoped<DateTimeOffsetHandler>();
        services.AddScoped<NullableDateTimeHandler>();
    }

    /// <summary>
    /// 擴展 Dapper SqlMapper 處理器
    /// </summary>
    /// <param name="app"></param>
    public static void AddDapperTypeHandler(this WebApplication app)
    {
        // 註冊 Dapper SqlMapper 處理器
        SqlMapper.AddTypeHandler(app.Services.GetRequiredService<DateTimeHandler>());
        SqlMapper.AddTypeHandler(app.Services.GetRequiredService<DateTimeOffsetHandler>());
        SqlMapper.AddTypeHandler(app.Services.GetRequiredService<NullableDateTimeHandler>());
    }
}
