using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure.Extension;

public static class DBContextExtension
{
    /// <summary>
    /// Add DBContext for Base
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddDBContextForBase(this IServiceCollection services, IConfiguration configuration) => services.AddDbContext<DbContext>(options => options.UseSqlServer(configuration.GetConnectionString("BaseConnection")));

    /// <summary>
    /// Execute with Dapper in EF Core, support transaction if enabled
    /// </summary>
    /// <param name="database">DatabaseFacade</param>
    /// <param name="commandText">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>The number of rows affected.</returns>
    public static int DapperExecute(
        this DatabaseFacade database,
        string commandText,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        System.Data.Common.DbConnection cn = database.GetDbConnection();
        IDbTransaction trn = database.CurrentTransaction?.GetDbTransaction()!;
        return cn.Execute(commandText, param, trn, commandTimeout, commandType);
    }

    /// <summary>
    /// Query with Dapper in EF Core, support transaction if enabled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="database">DatabaseFacade</param>
    /// <param name="commandText">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns></returns>
    public static IEnumerable<T> DapperQuery<T>(
        this DatabaseFacade database,
        string commandText,
        object param,
        bool buffered = true,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        System.Data.Common.DbConnection cn = database.GetDbConnection();
        IDbTransaction trn = database.CurrentTransaction?.GetDbTransaction()!;
        return cn.Query<T>(commandText, param, trn, buffered, commandTimeout, commandType);
    }
}
