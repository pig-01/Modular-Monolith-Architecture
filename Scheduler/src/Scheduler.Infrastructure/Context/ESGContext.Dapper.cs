using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;

namespace Scheduler.Infrastructure.Context;

public partial class DemoContext
{
    /// <summary>
    /// 取得可用的連線與 Transaction
    /// </summary>
    private (IDbConnection, IDbTransaction) GetConnectionAndTransaction()
    {
        if (Database.CurrentTransaction == null)
        {
            return (Database.GetDbConnection(), Database.BeginTransaction().GetDbTransaction());
        }
        return (Database.GetDbConnection(), Database.CurrentTransaction.GetDbTransaction());
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        return await conn.QueryAsync<T>(sql, param, tran);
    }

    public async Task<IEnumerable<T1>> QueryAsync<T1, T2>(string sql, Func<T1, T2, T1> func, object? param = null, string splitOn = "Id")
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        return await conn.QueryAsync(sql, func, param, tran, splitOn: splitOn);
    }

    public async Task<T> QueryFirstAsync<T>(string sql, object? param = null)
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        return await conn.QueryFirstAsync<T>(sql, param, tran);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        return await conn.QueryFirstOrDefaultAsync<T>(sql, param, tran);
    }

    public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param = null)
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        return await conn.QueryMultipleAsync(sql, param, tran);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null)
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        return await conn.QuerySingleAsync<T>(sql, param, tran);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        return await conn.QuerySingleOrDefaultAsync<T>(sql, param, tran);
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType? commandType = null)
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        int result = await conn.ExecuteAsync(sql, param, tran, null, commandType);
        return result;
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CommandType? commandType = null)
    {
        (IDbConnection conn, IDbTransaction tran) = GetConnectionAndTransaction();
        return await conn.ExecuteScalarAsync<T>(sql, param, tran, null, commandType);
    }
}
