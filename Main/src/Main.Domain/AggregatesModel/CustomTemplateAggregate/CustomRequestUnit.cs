using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Main.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Main.Domain.AggregatesModel.CustomTemplateAggregate;

/// <summary>
/// 自訂要求單位
/// </summary>
[Table("CustomRequestUnit")]
public partial class CustomRequestUnit : Entity, IAggregateRoot
{
    /// <summary>
    /// 要求單位識別碼
    /// </summary>
    [Key]
    [Column("UnitID")]
    public long UnitId { get; set; }

    /// <summary>
    /// 要求單位名稱
    /// </summary>
    [StringLength(255)]
    public string? UnitName { get; set; }

    /// <summary>
    /// 站台識別碼
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    [InverseProperty("Unit")]
    public virtual ICollection<CustomPlanTemplateVersion> CustomPlanTemplateVersions { get; set; } = [];

    /// <summary>
    /// 建立一個新的自訂要求單位
    /// </summary>
    /// <param name="unitName">要求單位名稱</param>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="createdUser">建立人員</param>
    public CustomRequestUnit(string unitName, string tenantId, string createdUser)
    {
        UnitName = unitName;
        TenantId = tenantId;
        SetCreateMetadata(createdUser, createdUser);
    }

    /// <summary>
    /// 新增一個版本
    /// </summary>
    /// <param name="version">版本名稱</param>
    /// <param name="createdUser">建立人員</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddVersion(string version, string createdUser)
    {
        if (CustomPlanTemplateVersions.Any(v => v.Version == version))
        {
            throw new InvalidOperationException($"Version {version} already exists for this CustomRequestUnit.");
        }
        CustomPlanTemplateVersion newVersion = new(version, createdUser);
        CustomPlanTemplateVersions.Add(newVersion);
    }

    /// <summary>
    /// 取得最新已發布的版本
    /// </summary>
    /// <returns></returns>
    public CustomPlanTemplateVersion? LastVersion => CustomPlanTemplateVersions is null || CustomPlanTemplateVersions.Count == 0
        ? null
        : CustomPlanTemplateVersions
        .Where(v => v.IsDeployed)
        .OrderByDescending(v => v.VersionId)
        .FirstOrDefault();
}