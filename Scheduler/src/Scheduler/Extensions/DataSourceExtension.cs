using Base.Infrastructure.Extension;
using Scheduler.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Scheduler.Extensions;

/// <summary>
/// 擴展資料來源
/// </summary>
public static class DataSourceExtension
{
        /// <summary>
        /// 擴展 DbContext
        /// </summary>
        /// <param name="builder"></param>
        public static void AddDbContext(this WebApplicationBuilder builder) =>
            // 加入 EF Core DbContext
            builder.Services.AddDbContext<DbContext, DemoContext>(options =>
            {
#if RELEASE
            // 使用 Azure SQL Database (生產環境)
            options.UseSqlServer(builder.Configuration.GetConnectionString("BdbuDemoPreProdAzureConnection")!.DecryptString(),
                    providerOptions => { });
#else
                    // 使用 SQL Server (開發環境)
                    options.UseSqlServer(builder.Configuration.GetConnectionString("BdbuDemoDev2022Connection")!.DecryptString(),
                        providerOptions => { });
#endif

                    // 設定 EF Core 日誌
                    options.EnableSensitiveDataLogging(false)
                    .EnableDetailedErrors(false);

            });
}
