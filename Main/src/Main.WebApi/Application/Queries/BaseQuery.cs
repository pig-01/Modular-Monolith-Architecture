using AutoMapper;
using Main.Dto.Model.Pagination;
using Main.Infrastructure.Demo.Context;
using static Dapper.SqlMapper;

namespace Main.WebApi.Application.Queries;

public class BaseQuery
{
    protected static async Task<PaginationResult<T>> PaginationResultBuilder<T>(DemoContext context, string paginationSql, object param)
    {
        // 執行分頁查詢
        GridReader gridReader = await context.QueryMultipleAsync(paginationSql, param);

        return await PaginationResultBuilder<T>(gridReader);
    }

    protected static async Task<PaginationResult<T>> PaginationResultBuilder<T>(GridReader gridReader)
    {
        // 取得查詢結果
        IEnumerable<T> list = await gridReader.ReadAsync<T>();
        int totalCount = await gridReader.ReadFirstAsync<int>();

        // 回傳分頁結果
        return new PaginationResult<T>(totalCount, [.. list]);
    }

    protected static async Task<PaginationResult<TResult>> PaginationResultBuilder<TRequest, TResponse, TResult>(SortedPaginationModel<TRequest> request, IQueryable<TResponse> list, IMapper mapper, CancellationToken cancellationToken = default)
    {
        // 取得查詢結果
        int totalCount = await list.CountAsync(cancellationToken);
        int pageCount = (int)Math.Ceiling((double)totalCount / request.PerPage);

        // 回傳分頁結果
        return new PaginationResult<TResult>(totalCount, mapper.Map<List<TResult>>(await list.Skip((request.Page - 1) * request.PerPage).Take(request.PerPage).ToListAsync(cancellationToken)));
    }
}
