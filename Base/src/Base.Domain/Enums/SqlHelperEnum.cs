using System.ComponentModel;

namespace Base.Domain.Enums;

public class SqlHelperEnum
{
    public enum JoinType
    {
        [Description("INNER JOIN")]
        InnerJoin,
        [Description("LEFT JOIN")]
        LeftJoin,
        [Description("RIGHT JOIN")]
        RightJoin,
        [Description("FULL JOIN")]
        FullJoin
    }
}
