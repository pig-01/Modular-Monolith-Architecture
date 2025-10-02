using System.ComponentModel;

namespace Scheduler.Domain.Enums;

public class TenantEnum(int id, string name) : Enumeration(id, name)
{
    /// <summary>
    /// 超級租戶
    /// </summary>
    /// <returns></returns>
    [Description("Super Tenant")]
    public static readonly TenantEnum SuperTenant = new(1, "SuperTenant");

    /// <summary>
    /// 自訂租戶
    /// </summary>
    /// <returns></returns>
    [Description("Custom Tenant")]
    public static readonly TenantEnum CustomTenant = new(2, "CustomTenant");
}