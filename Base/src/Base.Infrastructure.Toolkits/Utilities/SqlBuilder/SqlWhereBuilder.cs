namespace Base.Infrastructure.Toolkits.Utilities.SqlBuilder;

public class SqlWhereBuilder(SqlBuilder sqlBuilder)
{
    private readonly SqlBuilder sqlBuilder = sqlBuilder;

    public SqlWhereBuilder AddWhereClauseByCondition(string sqlWhereClause, bool condition)
    {
        if (condition)
        {
            sqlBuilder.AddWhereClause(sqlWhereClause);
        }

        return this;
    }

    public SqlWhereBuilder AddWhereClause(string sqlWhereClause)
    {
        sqlBuilder.AddWhereClause(sqlWhereClause);
        return this;
    }
}
