using System.Text;
using Base.Infrastructure.Toolkits.Extensions;
using static Base.Domain.Enums.SqlHelperEnum;

namespace Base.Infrastructure.Toolkits.Utilities.SqlBuilder;

public class SqlBuilder
{
    private readonly StringBuilder stringBuilder = new(string.Empty);


    public SqlBuilder() { }

    public SqlBuilder(string sql) => stringBuilder = new(TrimSql(sql));


    public SqlBuilder WhereScope(params Action<SqlWhereBuilder>[] where)
    {
        SqlWhereBuilder builder = new(this);

        return WhereScope(builder, where);
    }

    public SqlBuilder AddSQLWrap(string sqlFront, string sqlEnd)
    {
        stringBuilder.Insert(0, sqlFront);
        stringBuilder.Append(sqlEnd);
        return this;
    }

    private SqlBuilder WhereScope(SqlWhereBuilder whereBuilder, params Action<SqlWhereBuilder>[] where)
    {
        stringBuilder.Append(" WHERE (1 = 1) ");

        foreach (Action<SqlWhereBuilder> action in where)
        {
            action(whereBuilder);
        }

        return this;
    }

    public SqlBuilder AddOrderBy(string orderBy)
    {
        stringBuilder.Append(" ORDER BY " + orderBy);
        return this;
    }

    public SqlBuilder AddGroupBy(string groupBy)
    {
        stringBuilder.Append(" GROUP BY " + groupBy);
        return this;
    }

    public SqlBuilder AddHaving(string having)
    {
        stringBuilder.Append(" HAVING " + having);
        return this;
    }

    public SqlBuilder AddJoin(JoinType joinType, string join, string sqlJoinClause)
    {
        stringBuilder.Append($" {joinType.GetDescription()} {join} ON {sqlJoinClause}");
        return this;
    }

    public SqlBuilder AddJoin(string join, string sqlJoinClause)
    {
        stringBuilder.Append($" JOIN {join} ON {sqlJoinClause}");
        return this;
    }

    public SqlBuilder AddPagination(int page, int perPage)
    {
        int offset = (page - 1) * perPage;
        int fetch = perPage;

        stringBuilder.Append($" OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY ");
        stringBuilder.Append(GetCountSql());
        return this;
    }

    public SqlBuilder AddSortedPagination(int page, int perPage, string? sort)
    {
        int offset = (page - 1) * perPage;
        int fetch = perPage;

        if (sort is not null)
        {
            AddSQLWrap(" SELECT * FROM ( ", " ) Sql_Sorted_Pagin_Result ");
            AddOrderBy(sort);
        }

        stringBuilder.Append($" OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY ");
        stringBuilder.Append(GetCountSql());
        return this;
    }

    private static string TrimSql(string sql)
    {
        StringBuilder stringBuilder = new();
        string[] array = sql.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        stringBuilder.AppendJoin(" ", array.Select(x => x.Trim()));
        return stringBuilder.ToString();
    }

    private string GetCountSql()
    {
        string countSql = "SELECT COUNT(*) FROM ({0}) Ct";
        string sql = stringBuilder.ToString();
        int index = sql.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
        return string.Format(countSql, index == -1 ? sql : sql[..index]);
    }

    public override string ToString() => stringBuilder.ToString();

    public void AddWhereClause(string sqlWhereClause) => stringBuilder.Append(" " + sqlWhereClause);
}
