using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.AggregatesModel.OrderAggregate;
using DataHub.Domain.AggregatesModel.UserAggregate;
using DataHub.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace DataHub.Domain.AggregatesModel.TenantAggregate;

[Table("Tenant")]
public partial class Tenant : Entity, IAggregateRoot
{
    /// <summary>
    /// 站台識別碼
    /// </summary>
    [Key]
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// 站台名稱
    /// </summary>
    [StringLength(40)]
    public string TenantName { get; set; } = null!;

    [Column(TypeName = "image")]
    public byte[]? Logo { get; set; }

    /// <summary>
    /// 系統管理者帳號
    /// </summary>
    [Column("AccountID")]
    [StringLength(50)]
    public string AccountId { get; set; } = null!;

    /// <summary>
    /// 申請日期
    /// </summary>
    public DateOnly? ApplicationDate { get; set; }

    /// <summary>
    /// 申請人姓名
    /// </summary>
    [StringLength(50)]
    public string? Applicant { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [StringLength(100)]
    public string? Remark { get; set; }

    /// <summary>
    /// 站台起始日
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 站台結束日
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 站台公開識別碼
    /// </summary>
    [Column("IdentityID")]
    public Guid IdentityId { get; set; }

    [InverseProperty("Tenant")]
    public virtual ICollection<CurrentTenant> CurrentTenants { get; set; } = [];

    [InverseProperty("Tenant")]
    public virtual ICollection<OrderTenant> OrderTenants { get; set; } = [];

    [InverseProperty("Tenant")]
    public virtual ICollection<ScuserTenant> ScuserTenants { get; set; } = [];

    public static Tenant Create(string tenantId, string tenantName, string accountId, string application, byte[] logo, DateOnly applicationDate, DateTime startDate, DateTime endDate) => new()
    {
        TenantId = tenantId,
        TenantName = tenantName,
        AccountId = accountId,
        Applicant = application,
        ApplicationDate = applicationDate,
        StartDate = startDate,
        EndDate = endDate,
        IdentityId = Guid.NewGuid(),
        Logo = logo
    };
}
