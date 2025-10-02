using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.AggregatesModel.TenantAggregate;
using Microsoft.EntityFrameworkCore;

namespace DataHub.Domain.AggregatesModel.UserAggregate;

[PrimaryKey("UserId", "TenantId")]
[Table("CurrentTenant")]
[Index("UserId", Name = "UQ_CurrentTenant_UserID", IsUnique = true)]
[Index("UserId", "TenantId", Name = "UQ_CurrentTenant_UserID_TenantID", IsUnique = true)]
public partial class CurrentTenant
{
    [Key]
    [Column("UserID")]
    [StringLength(50)]
    public required string UserId { get; set; }

    [Key]
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public required string TenantId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("TenantId")]
    [InverseProperty("CurrentTenants")]
    public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("CurrentTenant")]
    public virtual Scuser User { get; set; } = null!;

    public static CurrentTenant Create(string tenantId, string userId) => new()
    {
        UserId = userId,
        TenantId = tenantId
    };
}
