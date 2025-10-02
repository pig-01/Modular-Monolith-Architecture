using System.ComponentModel;

namespace Scheduler.Domain.Enums;

public class DocumentStatus(int id, string name) : Enumeration(id, name)
{
    [Description("待填寫")]
    public static readonly DocumentStatus UnWritten = new(0, nameof(UnWritten));

    [Description("已填寫")]
    public static readonly DocumentStatus Written = new(1, nameof(Written));

    [Description("審核中")]
    public static readonly DocumentStatus Approving = new(2, nameof(Approving));

    [Description("退回")]
    public static readonly DocumentStatus SendBack = new(3, nameof(SendBack));

    [Description("取消")]
    public static readonly DocumentStatus Canceled = new(4, nameof(Canceled));

    [Description("通過")]
    public static readonly DocumentStatus Approved = new(5, nameof(Approved));

    [Description("不同意")]
    public static readonly DocumentStatus Rejected = new(6, nameof(Rejected));
}
